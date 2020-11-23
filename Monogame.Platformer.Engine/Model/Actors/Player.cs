using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Model.Actors.Abilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Model
{
    public class Player : Actor
    {
        private Map _map;
        private bool _onGround;
        private PointF _lastPosAdjustment;
        private RectangleF _lastPosition;
        private bool _jumpIsDisabled;
        private float _currentAcceleration = 0.001f;
        private float _accelerationIncrease = 0.15f;

        private bool _jumpButtonPressedDown = false;

        private WallJumping _wallJumpAbility;
        private bool _jumpInitiated;


        public Player(PointF startLocation, Map tmxMap)
        {
            Gravity = 600;
            Bounds = new RectangleF(startLocation, new SizeF(16, 16));
            Velocity = new Vector2(0, 0);
            Speed = new Vector2(10, 200);
            _map = tmxMap;

            _wallJumpAbility = new WallJumping();
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
        }

        public Vector2 GetPositionV()
        {
            return new Vector2(Bounds.X, Bounds.Y);
        }

        private void CalculateCollision()
        {
            var collisionRects = _map.GetObjectsInRegion(0, Utillities.Utilities.Round(Bounds));

            foreach (var collisionRect in collisionRects)
            {
                var isGroundCollision = Convert.ToInt32(_lastPosition.Bottom) <= collisionRect.Bounds.Top && Convert.ToInt32(Bounds.Bottom) >= collisionRect.Bounds.Top;
                var isRoofCollision = Convert.ToInt32(_lastPosition.Top) >= collisionRect.Bounds.Bottom && Convert.ToInt32(Bounds.Top) < collisionRect.Bounds.Bottom;
                var isRightCollision = Convert.ToInt32(_lastPosition.Right) <= collisionRect.Bounds.Left && Convert.ToInt32(Bounds.Right) >= collisionRect.Bounds.Left;
                var isLeftCollision = Convert.ToInt32(_lastPosition.Left) >= collisionRect.Bounds.Right && Convert.ToInt32(Bounds.Left) <= collisionRect.Bounds.Right;

                if (isGroundCollision && !_onGround)
                {
                    Bounds.Location = new PointF(_lastPosition.X, collisionRect.Bounds.Top - Bounds.Height);
                    _onGround = true;
                    _lastPosAdjustment = Bounds.Location;
                    Velocity.Y = Velocity.Y > 0 ? 0 : Velocity.Y;

                    _wallJumpAbility.ResetAbility();
                }
                else if (isRoofCollision)
                {
                    Velocity.Y = 0;
                    Bounds.Location = new PointF(_lastPosition.X, collisionRect.Bounds.Bottom);

                    _wallJumpAbility.ResetAbility();
                }
                else if (isRightCollision)
                {
                     Bounds.X = collisionRect.Bounds.Left - Bounds.Width;
                    _lastPosAdjustment.X = Bounds.X;

                    if (_wallJumpAbility.AbilityEnabled)
                        _wallJumpAbility.InitiateWallclinging(direction: WallJumping.Direct.Right);

                }
                else if (isLeftCollision)
                {
                    Bounds.X = collisionRect.Bounds.Right;
                    _lastPosAdjustment.X = Bounds.X;

                    if (_wallJumpAbility.AbilityEnabled)
                        _wallJumpAbility.InitiateWallclinging(direction: WallJumping.Direct.Left);
                }
            }

            // If no ordinary collisions are detected - do an additional check with a +1 inflated rectancle in
            // order to determine if the actor is positioned immediately next to a collision.
            if (!collisionRects.Any())
            {
                var colDetectionRect = Bounds;
                colDetectionRect.Inflate(1, 1);
                var collisionRectsInflateOne = _map.GetObjectsInRegion(0, colDetectionRect);

                if (!collisionRectsInflateOne.Any(x => (x.Bounds.Top == Bounds.Bottom)))
                {
                    _onGround = false;
                }

                if (_wallJumpAbility.AbilityEnabled)
                {
                    if (_wallJumpAbility.IsWallClinging && (!collisionRectsInflateOne.Any(x => x.Bounds.Left == Bounds.Right) && !collisionRectsInflateOne.Any(x => x.Bounds.Right == Bounds.Left)))
                    {
                        _wallJumpAbility.ResetWallClinging();
                    }
                }

                _jumpIsDisabled = collisionRectsInflateOne.Any(x => (x.Bounds.Bottom == Bounds.Top)); // Object bottom is colliding.
            }

        }

        private Vector2 CalculateMoveVelocity(Vector2 linearVelocity, Vector2 direction, Vector2 speed, float elapsedTime)
        {
            var newVelocity = linearVelocity;

            var jumpCanceled = newVelocity.Y < 0 && !_jumpButtonPressedDown && !_wallJumpAbility.IsWallJumping;
            var gravity = jumpCanceled ? Gravity * 3 : Gravity;
            var defaultVelocityY = newVelocity.Y += (_onGround ? 0 : gravity) * elapsedTime;

            if (_wallJumpAbility.AbilityEnabled)
            {
                var useDefaultYVelocity = false;
                newVelocity = _wallJumpAbility.CalculatVelocity(newVelocity, speed, _jumpInitiated, ref direction, ref _currentAcceleration, ref useDefaultYVelocity);            
                newVelocity.Y = useDefaultYVelocity ? defaultVelocityY : newVelocity.Y;
            }
            else
            {
                newVelocity.X = speed.X * direction.X;
                newVelocity.Y = defaultVelocityY;
            }

            // Standard single jump.
            if (_jumpInitiated  && !_jumpIsDisabled && !_wallJumpAbility.IsWallClinging)
            {
                newVelocity.Y = speed.Y * direction.Y;
                _onGround = false;
            }

            return newVelocity;
        }

        private Vector2 GetDirection(KeyboardState keyboardState)
        {
            var movingLeft = keyboardState.IsKeyDown(Keys.A) ? -Speed.X * _currentAcceleration : 0;
            var movingRight = keyboardState.IsKeyDown(Keys.D) ? -Speed.X * _currentAcceleration : 0;

            var jumpQueueIsOk = JumpQueueIsOk(keyboardState);
            _jumpInitiated = jumpQueueIsOk && (_onGround || _wallJumpAbility.IsWallClinging);

            return new Vector2(
                movingLeft - movingRight,
                _jumpInitiated ? -1 : 0f
            );
        }


        private bool JumpQueueIsOk(KeyboardState keyboardState)
        {
            var jumpEnabled = false;

            if (keyboardState.IsKeyDown(Keys.Space) && !_jumpButtonPressedDown)
            {
                _jumpButtonPressedDown = true;
                jumpEnabled = true;
            }

            if (keyboardState.IsKeyUp(Keys.Space))
            {
                _jumpButtonPressedDown = false;
               
            }

            return jumpEnabled;
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