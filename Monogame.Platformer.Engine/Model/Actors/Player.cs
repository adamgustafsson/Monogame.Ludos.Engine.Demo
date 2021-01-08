using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Model.Actors.Abilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model.Actors
{
    public class Player : Actor
    {
        private World.TMXManager _tmxManager;
        private RectangleF _lastPosition;
        
        private bool _onGround;
        private bool _imidiateTopCollisionExists;
        private bool _jumpButtonPressedDown = false;
        private bool _jumpInitiated;

        private bool _ladderIsAvailable;
        private MapObject _mostRecentLadder;

        private float _currentAcceleration = 0.001f;
        private float _accelerationIncrease = 0.15f;

        private bool _onMovingPlatform;

        public Player(PointF startLocation, World.TMXManager tmxManager)
        {
            Gravity = 600;
            Bounds = new RectangleF(startLocation, new SizeF(16, 24));
            Velocity = new Vector2(0, 0);
            Speed = new Vector2(10, Bounds.Y > 16 ? 225 : 200);
            _tmxManager = tmxManager;

            Abilities.AddRange(new List<IAbility>() { new WallJump(), new DoubleJump() });
            //GetAbility<DoubleJump>().AbilityEnabled = false;
        }

        public void Update(float elapsedTime, KeyboardState keyboardState)
        {
            _lastPosition = Bounds;

            var direction = GetDirection(keyboardState);
            Accelerate(ref direction);
            Velocity = CalculateMoveVelocity(Velocity, direction, Speed, elapsedTime);
           
            var currentPosition = new Vector2(Bounds.X, Bounds.Y);
            currentPosition += Velocity * elapsedTime;

            Bounds.X = currentPosition.X;
            Bounds.Y = currentPosition.Y;

            CalculateCollision();
            CalculateLadderCollision();
            CalculateMovingPlatformCollision();
            SetState();

            BottomDetectBounds = new RectangleF(Bounds.X, Bounds.Y + (Bounds.Height * 0.90f), Bounds.Width, Bounds.Height * 0.13f);

            if (GetAbility<DoubleJump>()?.AbilityEnabled ?? false)
            {
                if ((CurrentState == State.Jumping || CurrentState == State.Falling) && !GetAbility<DoubleJump>().DoubleJumpUsed)
                {
                    GetAbility<DoubleJump>().DoubleJumpAvailable = true;
                }
            }
        }

        public Vector2 GetPositionV()
        {
            return new Vector2(Bounds.X, Bounds.Y);
        }

        private void CalculateMovingPlatformCollision()
        {
            _onMovingPlatform = false;

            foreach (var mp in _tmxManager.MovingPlatforms)
            {
                var platformBounds = mp.Bounds;

                if (platformBounds.Intersects(Bounds))
                {
                    if (mp.DetectionBounds.Intersects(BottomDetectBounds) && !_jumpInitiated)
                    {
                        Bounds.Y = platformBounds.Top - Bounds.Height;
                        Bounds.Offset(mp.Change);
                        Velocity.Y = Velocity.Y > 0 ? 0 : Velocity.Y;
                        GetAbility<WallJump>()?.ResetAbility();
                        GetAbility<DoubleJump>()?.ResetAbility();
                        _onMovingPlatform = true;
                    }

                }
            }
        }

        private void CalculateCollision()
        {
            var collisionRects = _tmxManager.GetObjectsInRegion(World.DefaultLayerInfo.GROUND_COLLISION, Utillities.Utilities.Round(Bounds));

            foreach (var collisionRect in collisionRects.Where(x => x.Polyline == null))
            {
                var isGroundCollision = Convert.ToInt32(_lastPosition.Bottom) <= collisionRect.Bounds.Top && Convert.ToInt32(Bounds.Bottom) >= collisionRect.Bounds.Top;
                var isRoofCollision = Convert.ToInt32(_lastPosition.Top) >= collisionRect.Bounds.Bottom && Convert.ToInt32(Bounds.Top) < collisionRect.Bounds.Bottom;
                var isRightCollision = Convert.ToInt32(_lastPosition.Right) <= collisionRect.Bounds.Left && Convert.ToInt32(Bounds.Right) >= collisionRect.Bounds.Left;
                var isLeftCollision = Convert.ToInt32(_lastPosition.Left) >= collisionRect.Bounds.Right && Convert.ToInt32(Bounds.Left) <= collisionRect.Bounds.Right;

                if (isGroundCollision && !_onGround)
                {
                    SetGrounded(new PointF(_lastPosition.X, collisionRect.Bounds.Top - Bounds.Height));
                }
                else if (isRoofCollision)
                {
                    Velocity.Y = 0;
                    Bounds.Location = new PointF(_lastPosition.X, collisionRect.Bounds.Bottom);

                    GetAbility<WallJump>()?.ResetAbility();
                }
                else if (isRightCollision)
                {
                     Bounds.X = collisionRect.Bounds.Left - Bounds.Width;

                    if ((GetAbility<WallJump>()?.AbilityEnabled ?? false) && !_onGround)
                    {
                        GetAbility<WallJump>().InitiateWallclinging(direction: WallJump.ClingDir.Right);
                        GetAbility<DoubleJump>()?.ResetAbility();
                    }

                }
                else if (isLeftCollision)
                {
                    Bounds.X = collisionRect.Bounds.Right;

                    if ((GetAbility<WallJump>()?.AbilityEnabled ?? false) && !_onGround)
                    {
                        GetAbility<WallJump>().InitiateWallclinging(direction: WallJump.ClingDir.Left);
                        GetAbility<DoubleJump>()?.ResetAbility();
                    }
                }
            }

            // If no ordinary collisions are detected - do an additional check with a +1 inflated rectancle in
            // order to determine if the actor is positioned immediately next to a collision.
            if (!collisionRects.Any())
            {
                var colDetectionRect = Bounds;
                colDetectionRect.Inflate(0.2f, 0.2f);
                var collisionRectsInflateOne = _tmxManager.GetObjectsInRegion(World.DefaultLayerInfo.GROUND_COLLISION, colDetectionRect);

                if (!collisionRectsInflateOne.Any(x => (x.Bounds.Top == Bounds.Bottom)))
                {
                    _onGround = false;
                }

                if (GetAbility<WallJump>()?.AbilityEnabled ?? false)
                {
                    if (GetAbility<WallJump>().IsWallClinging && (!collisionRectsInflateOne.Any(x => x.Bounds.Left == Bounds.Right) && !collisionRectsInflateOne.Any(x => x.Bounds.Right == Bounds.Left)))
                    {
                        GetAbility<WallJump>().ResetWallClinging();
                    }
                }

                _imidiateTopCollisionExists = collisionRectsInflateOne.Any(x => (x.Bounds.Bottom == Bounds.Top)); // Object bottom is colliding.
            }
        }

        private void CalculateLadderCollision()
        {
            var ladderDetectionBounds = Bounds;
            ladderDetectionBounds.Inflate(-(Bounds.Width * 0.60f), 0f);

            var onTopOfLadder = false;

            if (_mostRecentLadder != null)
            {
                onTopOfLadder = (ladderDetectionBounds.Right <= _mostRecentLadder.Bounds.Right) &&
                    (ladderDetectionBounds.Left >= _mostRecentLadder.Bounds.Left) &&
                    (Convert.ToInt32(_lastPosition.Bottom) <= _mostRecentLadder.Bounds.Top && Convert.ToInt32(Bounds.Bottom) >= _mostRecentLadder.Bounds.Top);

                if (onTopOfLadder)
                {
                    SetGrounded(new PointF(Bounds.X, _mostRecentLadder.Bounds.Top - Bounds.Height));
                }
            }

            var ladders = _tmxManager.GetObjectsInRegion(World.DefaultLayerInfo.INTERACTABLE_OBJECTS, ladderDetectionBounds, new KeyValuePair<string, string>("type", "ladder"));
            _ladderIsAvailable = ladders.Any();

            if (!_ladderIsAvailable)
            {
                OnLadder = false;
            }
            else if (OnLadder)
            {
                _mostRecentLadder = ladders.ToList()[0];

                if (!(_onGround && !onTopOfLadder))
                {
                    Bounds.X = _mostRecentLadder.Bounds.X;
                }

                if (onTopOfLadder)
                {
                    Bounds.Y = Bounds.Y + (Bounds.Height / 4);
                }
            }
        }

        private void SetGrounded(PointF currentPosition)
        {
            Bounds.Location = currentPosition;
            _onGround = true;
            Velocity.Y = Velocity.Y > 0 ? 0 : Velocity.Y;

            GetAbility<WallJump>()?.ResetAbility();
            GetAbility<DoubleJump>()?.ResetAbility();
        }

        private Vector2 CalculateMoveVelocity(Vector2 linearVelocity, Vector2 direction, Vector2 speed, float elapsedTime)
        {
            var newVelocity = linearVelocity;

            var jumpCanceled = newVelocity.Y < 0 && !_jumpButtonPressedDown && !(GetAbility<WallJump>()?.IsWallJumping ?? false);
            var gravity = jumpCanceled ? Gravity * 3 : Gravity;
            var defaultVelocityY = newVelocity.Y += (_onGround ? 0 : gravity) * elapsedTime;

            if (GetAbility<WallJump>()?.AbilityEnabled ?? false)
            {
                var useDefaultYVelocity = false;
                newVelocity = GetAbility<WallJump>().CalculatVelocity(newVelocity, speed, _jumpInitiated, ref direction, ref _currentAcceleration, ref useDefaultYVelocity);            
                newVelocity.Y = useDefaultYVelocity ? defaultVelocityY : newVelocity.Y;
            }
            else
            {
                newVelocity.X = speed.X * direction.X;
                newVelocity.Y = defaultVelocityY;
            }

            // Standard single jump.
            if (_jumpInitiated  && !_imidiateTopCollisionExists && !(GetAbility<WallJump>()?.IsWallClinging ?? false))
            {
                newVelocity.Y = speed.Y * direction.Y;
                _onGround = false;
                OnLadder = false;
            } 
            if (OnLadder)
            {
                newVelocity.Y = speed.X * direction.Y;

                if (_onGround && newVelocity.Y > 0)
                {
                    newVelocity.Y = 0;
                }
            }

            return newVelocity;
        }

        private Vector2 GetDirection(KeyboardState keyboardState)
        {
            var movingLeft = keyboardState.IsKeyDown(Keys.A) ? -Speed.X * _currentAcceleration : 0;
            var movingRight = keyboardState.IsKeyDown(Keys.D) ? -Speed.X * _currentAcceleration : 0;

            var climbingUp = keyboardState.IsKeyDown(Keys.W) && _ladderIsAvailable ? -Speed.X : 0;
            var climbingDown = keyboardState.IsKeyDown(Keys.S) && _ladderIsAvailable ? -Speed.X : 0;

            var jumpQueueIsOk = JumpQueueIsOk(keyboardState);
            _jumpInitiated = jumpQueueIsOk && (_onGround || OnLadder || _onMovingPlatform || (GetAbility<WallJump>()?.IsWallClinging ?? false) || (GetAbility<DoubleJump>()?.DoubleJumpAvailable ?? false));

            if (_jumpInitiated && !_onGround && !OnLadder && !_onMovingPlatform && !(GetAbility<WallJump>()?.IsWallClinging ?? false))
            {
                GetAbility<DoubleJump>().DoubleJumpAvailable = false;
                GetAbility<DoubleJump>().DoubleJumpUsed = true;
            }

            var climbingDirection = climbingUp - climbingDown;

            if (!OnLadder && climbingDirection != 0)
            {
                OnLadder = true;
                GetAbility<DoubleJump>()?.ResetAbility();
            }

            return new Vector2(
                movingLeft - movingRight,
                _jumpInitiated ? -1 : climbingDirection
            );
        }


        private bool JumpQueueIsOk(KeyboardState keyboardState)
        {
            var jumpAvailable = false;

            if (keyboardState.IsKeyDown(Keys.Space) && !_jumpButtonPressedDown)
            {
                _jumpButtonPressedDown = true;
                jumpAvailable = true;
            }

            if (keyboardState.IsKeyUp(Keys.Space))
            {
                _jumpButtonPressedDown = false;
               
            }

            return jumpAvailable;
        }

        private void Accelerate(ref Vector2 direction)
        {
            direction.X = direction.X > Speed.X ? Speed.X : direction.X;
            direction.X = direction.X < -Speed.X ? -Speed.X : direction.X;

            var acceleratingRight = direction.X > 0 && _currentAcceleration < 1;
            var acceleratingLeft = direction.X < 0 && _currentAcceleration < 1;

            if (acceleratingRight || acceleratingLeft)
                _currentAcceleration += _accelerationIncrease;
            else if (direction.X == 0)
                _currentAcceleration = 0.001f;
        }
    }
}