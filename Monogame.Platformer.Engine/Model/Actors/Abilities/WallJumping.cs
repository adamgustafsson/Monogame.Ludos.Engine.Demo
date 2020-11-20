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
