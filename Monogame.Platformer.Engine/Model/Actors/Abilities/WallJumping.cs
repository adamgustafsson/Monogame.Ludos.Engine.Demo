using System.Diagnostics;


namespace Monogame.Platformer.Engine.Model
{
    internal class WallJumping
    {
        public bool WallJumpEnabled;
        public bool IsWallClinging;
        public bool IsWallJumping;
        public Stopwatch WallJumpTimer;
        public Stopwatch WallClingMovementCooldown;
        public int Direction;
    }
}
