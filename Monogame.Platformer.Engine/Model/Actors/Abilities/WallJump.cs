using Microsoft.Xna.Framework;
using System.Diagnostics;


namespace Model.Actors.Abilities
{
    internal class WallJump : IAbility
    {
        public bool IsWallClinging { get; set; }
        public bool IsWallJumping { get; set; }
        public Stopwatch WallJumpTimer { get; set; }
        public Stopwatch WallClingReleaseCd { get; set; }
        public ClingDir ClingDirection { get; set; }
        public bool AbilityEnabled { get; set; }

        public enum ClingDir
        {
            Right = 1,
            Left = -1
        }

        public WallJump()
        {
            AbilityEnabled = true;
            WallClingReleaseCd = new Stopwatch();
            WallJumpTimer = new Stopwatch();
        }

        public Vector2 CalculatVelocity(Vector2 currentVelocity, Vector2 actorSpeed, bool jumpInitiated, ref Vector2 direction, ref float currentAcceleration, ref bool useDefaultYVelocity)
        {
            var originalXdirection = direction.X;

            var isReleasingWallLeftwards = direction.X < 0 && ClingDirection == WallJump.ClingDir.Right;
            var isRealeasingWallRightwards = direction.X > 0 && ClingDirection == WallJump.ClingDir.Left;

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

                if (jumpInitiated)
                {
                    IsWallJumping = true;

                    if ((ClingDirection == WallJump.ClingDir.Right && originalXdirection < 0) || (ClingDirection == WallJump.ClingDir.Left && originalXdirection > 0))
                    {
                        currentVelocity.Y = (actorSpeed.Y * 1.25f) * direction.Y;
                    }
                    else if (originalXdirection != 0)
                    {
                        currentVelocity.Y = actorSpeed.Y * direction.Y;
                    }

                    currentVelocity.X = ClingDirection == WallJump.ClingDir.Right ? -25 : 25;
                    WallJumpTimer.Start();
                }
            }
            else
            {
                useDefaultYVelocity = true;
            }

            return currentVelocity;
        }

        public void InitiateWallclinging(ClingDir direction)
        {
            IsWallClinging = true;
            IsWallJumping = false;
            ClingDirection = direction;
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