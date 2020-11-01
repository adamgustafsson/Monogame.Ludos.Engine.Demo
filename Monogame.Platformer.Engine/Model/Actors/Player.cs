using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Monogame.Platformer.Engine.Model
{
    class Player : Actor
    {

        private Map _map;
        private Boolean _onGround = false;
        private float _gravity = 600;
        private Point _lastPosAdjustment;
        private Point _lastPosition;
        private Boolean _hitRoof;

        public Player(Point startLocation, Map tmxMap)
        {
            Gravity = _gravity;
            PlayerBounds = new Rectangle(startLocation, new Point(16, 16));
            Velocity = new Vector2(0, 0);
            Speed = new Vector2(10, 200);
            _map = tmxMap;
        }

        public void Update(float elapsedTime, KeyboardState keyboardState)
        {
            Gravity = _onGround ? 0 : _gravity;
            _lastPosition = new Point(PlayerBounds.X, PlayerBounds.Y);
                      
            var direction = GetDirection(keyboardState);  
            Velocity = CalculateMoveVelocity(Velocity, direction, Speed, elapsedTime);
            
            var currentPosition = new Vector2(PlayerBounds.X, PlayerBounds.Y);
            currentPosition += Velocity * elapsedTime;

            PlayerBounds.X = (Convert.ToInt32(currentPosition.X));
            PlayerBounds.Y = (Convert.ToInt32(currentPosition.Y));

            CheckVerticalCollision();
        }

        private void CheckVerticalCollision()
        {

            var colDetectionRect = PlayerBounds;
            colDetectionRect.Inflate(1, 1);

            var collisionRects = _map.GetObjectsInRegion(0, PlayerBounds);
            var collisionRectsInflateOne = _map.GetObjectsInRegion(0, colDetectionRect);

            foreach (var collisionRect in collisionRects)
            {
                var isGroundCollision = _lastPosition.Y < collisionRect.Bounds.Top && PlayerBounds.Y <= collisionRect.Bounds.Top;
                var isRoofCollision = _lastPosition.Y > collisionRect.Bounds.Bottom && PlayerBounds.Y <= collisionRect.Bounds.Bottom;

                if (isGroundCollision && !_onGround)
                {
                    SetPosition(_lastPosition.X, collisionRect.Bounds.Top - PlayerBounds.Height);
                    _onGround = true;
                    _hitRoof = false;
                    _lastPosAdjustment = GetPosition();

                    if (Velocity.Y > 0)
                    {
                        Velocity.Y = 0;
                    }

                }
                else if (isRoofCollision)
                {
                    Velocity.Y = 0;
                    SetPosition(_lastPosition.X, collisionRect.Bounds.Bottom);
                }
                //var isRightCollision = (PlayerBounds.Right >= collisionRect.Bounds.Left) && collisionRect.Bounds.Top < PlayerBounds.Bottom;

                //if (isRightCollision)
                //{
                //    PlayerBounds.X = collisionRect.Bounds.Left - PlayerBounds.Width;
                //    _lastPosAdjustment.X = PlayerBounds.X;

                //    if (PlayerBounds.Bottom < collisionRect.Bounds.Bottom)
                //    {
                //        Velocity.Y = 0;
                //    }

                //}
            }
            //}

            var positionAdjustedForGround = !collisionRects.Any() && _lastPosAdjustment.Y == PlayerBounds.Y;
            var noGroundDetected = !collisionRectsInflateOne.Any();

            if (positionAdjustedForGround && noGroundDetected)
            {
                _onGround = false;
            }
        }


        private Vector2 CalculateMoveVelocity(Vector2 linearVelocity, Vector2 direction, Vector2 speed, float elapsedTime)
        {
            var newVelocity = linearVelocity;
            newVelocity.X = speed.X * direction.X;
            newVelocity.Y += Gravity * elapsedTime;

            if (direction.Y == -1f && !_hitRoof)
            {
                newVelocity.Y = speed.Y * direction.Y;
                _onGround = false;
            }

            //if (isJumpInterupted)
            //{
            //    newVelocity.y = 0f;
            //}

            return newVelocity;
        }

        private Vector2 GetDirection(KeyboardState keyboardState)
        {
            var movingLeft = keyboardState.IsKeyDown(Keys.A) ? -Speed.X : 0;
            var movingRight = keyboardState.IsKeyDown(Keys.D) ? -Speed.X : 0;

            return new Vector2(
                movingLeft - movingRight,
                keyboardState.IsKeyDown(Keys.Space) && _onGround ? -1.0f : 0f
            );
        }

        private void SetPosition(Point position)
        {
            SetPosition(position.X, position.Y);
        }

        private void SetPosition(int xPos, int yPos)
        {
            PlayerBounds.X = xPos;
            PlayerBounds.Y = yPos;
        }

        private Point GetPosition()
        {
            return new Point(PlayerBounds.X, PlayerBounds.Y);
        }

        public Vector2 GetPositionAsVector()
        {
            return new Vector2(PlayerBounds.X, PlayerBounds.Y);
        }
    }
}