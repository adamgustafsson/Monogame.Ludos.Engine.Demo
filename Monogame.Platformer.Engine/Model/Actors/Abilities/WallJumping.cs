using Microsoft.Xna.Framework;
using System.Diagnostics;


namespace Monogame.Platformer.Engine.Model
{
    internal class WallJumping
    {
        public bool AbilityEnabled;
        public bool IsWallClinging;
        public bool IsWallJumping;
        public Stopwatch WallJumpTimer;
        public Stopwatch WallClingReleaseCd;
        public Direct Direction;

        public enum Direct
        {
            Right = 1,
            Left = -1
        }

        public WallJumping()
        {
            AbilityEnabled = true;
            WallClingReleaseCd = new Stopwatch();
            WallJumpTimer = new Stopwatch();
        }

        public Vector2 CalculatVelocity(Vector2 currentVelocity, Vector2 actorSpeed, ref Vector2 direction, ref float currentAcceleration, ref bool useDefaultYVelocity)
        {
            var originalXdirection = direction.X;

            var isReleasingWallLeftwards = direction.X < 0 && Direction == WallJumping.Direct.Right;
            var isRealeasingWallRightwards = direction.X > 0 && Direction == WallJumping.Direct.Left;

            // Short cooldown for x and y movement when starting to cling against a wall.
            direction.X = IsWallClinging && WallClingReleaseCd.ElapsedMilliseconds < 500 ? 0 : originalXdirection;

            if (IsWallClinging && (isReleasingWallLeftwards || isRealeasingWallRightwards))
            {
                WallClingReleaseCd.Start();
            }

            if (!IsWallJumping)
            {
                currentVelocity.X = actorSpeed.X * direction.X;
            }
            else if (IsWallJumping && (isReleasingWallLeftwards || isRealeasingWallRightwards))
            {
                ResetWallJumping();
            }
            else if (IsWallJumping && WallJumpTimer.ElapsedMilliseconds > 150)
            {
                currentAcceleration = 0.0001f;
                ResetWallJumping();
            }

            if (IsWallClinging && currentVelocity.Y >= 0)
            {
                currentVelocity.Y = 15;

                if (direction.Y == -1f)
                {
                    IsWallJumping = true;

                    if ((Direction == WallJumping.Direct.Right && originalXdirection < 0) || (Direction == WallJumping.Direct.Left && originalXdirection > 0))
                    {
                        currentVelocity.Y = (actorSpeed.Y * 1.25f) * direction.Y;
                    }
                    else if (originalXdirection != 0)
                    {
                        currentVelocity.Y = actorSpeed.Y * direction.Y;
                    }

                    currentVelocity.X = Direction == WallJumping.Direct.Right ? -25 : 25;
                    WallJumpTimer.Start();
                }
            }
            else
            {
                useDefaultYVelocity = true;
            }

            return currentVelocity;
        }

        public void InitiateWallclinging(Direct direction)
        {
            IsWallClinging = true;
            IsWallJumping = false;
            Direction = direction;
        }

        public void ResetWallClinging()
        {
            IsWallClinging = false;
            WallClingReleaseCd.Reset();
        }

        public void ResetWallJumping()
        {
            IsWallJumping = false;
            WallJumpTimer.Reset();
        }

        public void ResetAbility()
        {
            ResetWallClinging();
            ResetWallJumping();

        }
    }
}


//var originalXdirection = direction.X;

//var isReleasingWallLeftwards = direction.X < 0 && Direction == WallJumping.Direct.Right;
//var isRealeasingWallRightwards = direction.X > 0 && Direction == WallJumping.Direct.Left;

//if (IsWallClinging && (isReleasingWallLeftwards || isRealeasingWallRightwards))
//{
//    WallClingReleaseCd.Start();
//}

//if (!IsWallJumping && IsWallClinging)
//{
//    // Short cooldown for x and y movement when starting to cling against a wall.
//    currentVelocity.X = actorSpeed.X * WallClingReleaseCd.ElapsedMilliseconds < 500 ? 0 : originalXdirection;
//}
//else if (IsWallJumping && (isReleasingWallLeftwards || isRealeasingWallRightwards))
//{
//    ResetWallJumping();
//    currentVelocity.X = prevVelocity.X;
//}
//else if (IsWallJumping && WallJumpTimer.ElapsedMilliseconds > 150)
//{
//    currentAcceleration = 0.0001f;
//    ResetWallJumping();
//    currentVelocity.X = prevVelocity.X;
//}

//var actorIsNotJumping = currentVelocity.Y >= 0;

//if (IsWallClinging && actorIsNotJumping)
//{
//    currentVelocity.Y = 15;

//    if (direction.Y == -1f)
//    {
//        IsWallJumping = true;

//        if ((Direction == WallJumping.Direct.Right && originalXdirection < 0) || (Direction == WallJumping.Direct.Left && originalXdirection > 0))
//        {
//            currentVelocity.Y = (actorSpeed.Y * 1.25f) * direction.Y;
//        }
//        else if (originalXdirection != 0)
//        {
//            currentVelocity.Y = actorSpeed.Y * direction.Y;
//        }

//        currentVelocity.X = Direction == WallJumping.Direct.Right ? -25 : 25;
//        WallJumpTimer.Start();
//    }
//}

//return currentVelocity;