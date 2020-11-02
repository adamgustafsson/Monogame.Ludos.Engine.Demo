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
        private bool _onGround;
        private Point _lastPosAdjustment;
        private Rectangle _lastPosition;
        private bool _jumpIsDisabled;

        public Player(Point startLocation, Map tmxMap)
        {
            Gravity = 600;
            Bounds = new Rectangle(startLocation, new Point(16, 16));
            Velocity = new Vector2(0, 0);
            Speed = new Vector2(10, 200);
            _map = tmxMap;
        }

        public void Update(float elapsedTime, KeyboardState keyboardState)
        {
            _lastPosition = Bounds;
                      
            var direction = GetDirection(keyboardState);  
            Velocity = CalculateMoveVelocity(Velocity, direction, Speed, elapsedTime);
            
            var currentPosition = new Vector2(Bounds.X, Bounds.Y);
            currentPosition += Velocity * elapsedTime;

            Bounds.X = (Convert.ToInt32(currentPosition.X));
            Bounds.Y = (Convert.ToInt32(currentPosition.Y));

            CheckVerticalCollision();
        }

        public Vector2 GetPositionV()
        {
            return new Vector2(Bounds.X, Bounds.Y);
        }

        private void CheckVerticalCollision()
        {


            var collisionRects = _map.GetObjectsInRegion(0, Bounds);


            foreach (var collisionRect in collisionRects)
            {
                var isGroundCollision = _lastPosition.Bottom <= collisionRect.Bounds.Top && Bounds.Bottom >= collisionRect.Bounds.Top;
                var isRoofCollision = _lastPosition.Top >= collisionRect.Bounds.Bottom && Bounds.Top <= collisionRect.Bounds.Bottom;
                var isRightCollision = _lastPosition.Right <= collisionRect.Bounds.Left && Bounds.Right >= collisionRect.Bounds.Left;
                var isLeftCollision = _lastPosition.Left >= collisionRect.Bounds.Right && Bounds.Left <= collisionRect.Bounds.Right;

                if (isGroundCollision && !_onGround)
                {
                    SetPosition(_lastPosition.X, collisionRect.Bounds.Top - Bounds.Height);
                    _onGround = true;
                    _lastPosAdjustment = GetPosition();
                    Velocity.Y = Velocity.Y > 0 ? 0 : Velocity.Y;
                }
                else if (isRoofCollision)
                {
                    Velocity.Y = 0;
                    SetPosition(_lastPosition.X, collisionRect.Bounds.Bottom);
                }
                else if (isRightCollision)
                {
                    Bounds.X = collisionRect.Bounds.Left - Bounds.Width;
                    _lastPosAdjustment.X = Bounds.X;
                }
                else if (isLeftCollision)
                {
                    Bounds.X = collisionRect.Bounds.Right;
                    _lastPosAdjustment.X = Bounds.X;
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

                _jumpIsDisabled = collisionRectsInflateOne.Any(x => (x.Bounds.Bottom == Bounds.Top)); // Object bottom is colliding.
            }

        }

        private Vector2 CalculateMoveVelocity(Vector2 linearVelocity, Vector2 direction, Vector2 speed, float elapsedTime)
        {
            var newVelocity = linearVelocity;
            newVelocity.X = speed.X * direction.X;
            newVelocity.Y += (_onGround ? 0: Gravity) * elapsedTime;

            if (direction.Y == -1f && !_jumpIsDisabled)
            {
                newVelocity.Y = speed.Y * direction.Y;
                _onGround = false;
            }
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

        private void SetPosition(int xPos, int yPos)
        {
            Bounds.X = xPos;
            Bounds.Y = yPos;
        }

        private Point GetPosition()
        {
            return new Point(Bounds.X, Bounds.Y);
        }
    }
}