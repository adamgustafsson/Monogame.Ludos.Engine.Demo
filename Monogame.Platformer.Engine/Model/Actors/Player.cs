using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Net.WebSockets;

namespace Monogame.Platformer.Engine.Model
{
    class Player : Actor
    {

        private Map _map;
        private Boolean _onGround = false;
        private float _gravity = 600;
        private Point _lastPosAdjustment;
        private Rectangle _lastPosition;
        private Boolean _jumpIsDisabled;

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
            _lastPosition = PlayerBounds;
                      
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
                var isGroundCollision = _lastPosition.Bottom <= collisionRect.Bounds.Top && PlayerBounds.Bottom >= collisionRect.Bounds.Top;
                var isRoofCollision = _lastPosition.Top >= collisionRect.Bounds.Bottom && PlayerBounds.Top <= collisionRect.Bounds.Bottom;
                var isRightCollision = _lastPosition.Right <= collisionRect.Bounds.Left && PlayerBounds.Right >= collisionRect.Bounds.Left;
                var isLeftCollision = _lastPosition.Left >= collisionRect.Bounds.Right && PlayerBounds.Left <= collisionRect.Bounds.Right;

                if (isGroundCollision && !_onGround)
                {
                    SetPosition(_lastPosition.X, collisionRect.Bounds.Top - PlayerBounds.Height);
                    _onGround = true;
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
                else if (isRightCollision)
                {
                    PlayerBounds.X = collisionRect.Bounds.Left - PlayerBounds.Width;
                    _lastPosAdjustment.X = PlayerBounds.X;
                }
                else if (isLeftCollision)
                {
                    PlayerBounds.X = collisionRect.Bounds.Right;
                    _lastPosAdjustment.X = PlayerBounds.X;
                }
            }
            //}

            var positionAdjustedForGround = !collisionRects.Any() && _lastPosAdjustment.Y == PlayerBounds.Y;
            var noGroundDetected = !collisionRectsInflateOne.Any(x => x.Bounds.Top >= PlayerBounds.Bottom);

            if (positionAdjustedForGround && noGroundDetected)
            {
                _onGround = false;
            }

            //_jumpIsDisabled = collisionRectsInflateOne.Any(x => (x.Bounds.Bottom == PlayerBounds.Top + 1)); // Object bottom is colliding.
        }


        private Vector2 CalculateMoveVelocity(Vector2 linearVelocity, Vector2 direction, Vector2 speed, float elapsedTime)
        {
            var newVelocity = linearVelocity;
            newVelocity.X = speed.X * direction.X;
            newVelocity.Y += Gravity * elapsedTime;

            if (direction.Y == -1f && !_jumpIsDisabled)
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