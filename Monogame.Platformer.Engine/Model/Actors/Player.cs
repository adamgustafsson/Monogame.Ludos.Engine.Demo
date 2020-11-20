﻿using FuncWorks.XNA.XTiled;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Monogame.Platformer.Engine.Model
{
    class Player : Actor
    {
        private Map _map;
        private bool _onGround;
        private PointF _lastPosAdjustment;
        private RectangleF _lastPosition;
        private bool _jumpIsDisabled;
        private float _currentAcceleration = 0.001f;
        private float _accelerationIncrease = 0.15f;

        //private bool _wallJumpEnabled = true;
        //private bool _isWallClinging;
        //private bool _isWallJumping;
        //private Stopwatch _wallJumpTimer;
        //private Stopwatch _wallClingMovementCooldown;

        private WallJumping _wallJumpAbility;

        public Player(PointF startLocation, Map tmxMap)
        {
            Gravity = 600;
            Bounds = new RectangleF(startLocation, new SizeF(16, 16));
            Velocity = new Vector2(0, 0);
            Speed = new Vector2(10, 200);
            _map = tmxMap;
            //_wallJumpTimer = new Stopwatch();
            //_wallClingMovementCooldown = new Stopwatch();

            _wallJumpAbility = new WallJumping
            {
                WallJumpEnabled = true,
                WallClingMovementCooldown = new Stopwatch(),
                WallJumpTimer = new Stopwatch()
            };
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

                    _wallJumpAbility.IsWallClinging = false;
                    _wallJumpAbility.IsWallJumping = false;
                    _wallJumpAbility.WallClingMovementCooldown.Reset();

                    //_isWallClinging = false;
                    //_isWallJumping = false;
                    //_wallClingMovementCooldown.Reset();

                    _lastPosAdjustment = Bounds.Location;
                    Velocity.Y = Velocity.Y > 0 ? 0 : Velocity.Y;
                }
                else if (isRoofCollision)
                {
                    Velocity.Y = 0;
                    Bounds.Location = new PointF(_lastPosition.X, collisionRect.Bounds.Bottom);
                    //_isWallJumping = false;
                    _wallJumpAbility.IsWallJumping = false;
                }
                else if (isRightCollision)
                {
                     Bounds.X = collisionRect.Bounds.Left - Bounds.Width;
                    _lastPosAdjustment.X = Bounds.X;
                    //_isWallClinging = _wallJumpEnabled;
                    //_isWallJumping = false;
                    //_wallClingMovementCooldown.Start();

                    _wallJumpAbility.IsWallClinging = _wallJumpAbility.WallJumpEnabled;
                    _wallJumpAbility.IsWallJumping = false;
                    _wallJumpAbility.WallClingMovementCooldown.Start();
                    _wallJumpAbility.Direction = 1;

                }
                else if (isLeftCollision)
                {
                    Bounds.X = collisionRect.Bounds.Right;
                    _lastPosAdjustment.X = Bounds.X;
                    //_isWallClinging = _wallJumpEnabled;
                    //_isWallJumping = false;
                    //_wallClingMovementCooldown.Start();
                    _wallJumpAbility.IsWallClinging = _wallJumpAbility.WallJumpEnabled;
                    _wallJumpAbility.IsWallJumping = false;
                    _wallJumpAbility.WallClingMovementCooldown.Start();
                    _wallJumpAbility.Direction = -1;
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

                if (_wallJumpAbility.IsWallClinging && (!collisionRectsInflateOne.Any(x => x.Bounds.Left == Bounds.Right) && !collisionRectsInflateOne.Any(x => x.Bounds.Right == Bounds.Left)))
                {
                    _wallJumpAbility.IsWallClinging = false;
                    _wallJumpAbility.WallClingMovementCooldown.Reset();
                }

                _jumpIsDisabled = collisionRectsInflateOne.Any(x => (x.Bounds.Bottom == Bounds.Top)); // Object bottom is colliding.
            }

        }

        private Vector2 CalculateMoveVelocity(Vector2 linearVelocity, Vector2 direction, Vector2 speed, float elapsedTime)
        {
            var newVelocity = linearVelocity;
            var originalXdirection = direction.X;

            // Short cooldown for x and y movement when starting to cling against a wall.
            direction.X = _wallJumpAbility.IsWallClinging && _wallJumpAbility.WallClingMovementCooldown.ElapsedMilliseconds < 500 ? 0 : originalXdirection;
            var isWalljumpingLeft = direction.X < 0 && _wallJumpAbility.Direction == 1;
            var isWalljumpingRight = direction.X > 0 && _wallJumpAbility.Direction == -1;

            if (!_wallJumpAbility.IsWallJumping)
            {
                newVelocity.X = speed.X * direction.X;
            }
            else if (_wallJumpAbility.IsWallJumping && (isWalljumpingLeft || isWalljumpingRight))
            {
                _wallJumpAbility.IsWallJumping = false;
                _wallJumpAbility.WallJumpTimer.Reset();
            }
            else if (_wallJumpAbility.IsWallJumping && _wallJumpAbility.WallJumpTimer.ElapsedMilliseconds > 150)
            {
                _currentAcceleration = 0.0001f;
                _wallJumpAbility.IsWallJumping = false;
                _wallJumpAbility.WallJumpTimer.Reset();
            }


            if (_wallJumpAbility.IsWallClinging && newVelocity.Y >= 0)
            {
                newVelocity.Y = 15;

                if (direction.Y == -1f)
                {
                    _wallJumpAbility.IsWallJumping = true;

                    if ((_wallJumpAbility.Direction == 1 && originalXdirection < 0) || (_wallJumpAbility.Direction == -1 && originalXdirection > 0))
                    {
                        newVelocity.Y = (speed.Y * 1.25f) * direction.Y;
                    }
                    else if (originalXdirection != 0)
                    {
                        newVelocity.Y = speed.Y * direction.Y;
                    }

                    newVelocity.X = _wallJumpAbility.Direction == 1 ? -25 : 25;

                    //if (_wallJumpAbility.Direction == 1)
                    //{
                    //    newVelocity.X = -25;
                    //}
                    //else if (_wallJumpAbility.Direction == -1)
                    //{
                    //    newVelocity.X = 25;
                    //}

                    _wallJumpAbility.WallJumpTimer.Start();
                }
            }
            else
            {
                // Standard.
                newVelocity.Y += (_onGround ? 0 : Gravity) * elapsedTime;
            }



            if (direction.Y == -1f && !_jumpIsDisabled && !_wallJumpAbility.IsWallClinging)
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

            return new Vector2(
                movingLeft - movingRight,
                keyboardState.IsKeyDown(Keys.Space) && (_onGround || _wallJumpAbility.IsWallClinging) ? -1.0f : 0f
            );
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