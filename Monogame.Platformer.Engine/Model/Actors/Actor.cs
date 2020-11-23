using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Drawing;

namespace Model.Actors
{
    public class Actor
    {
        public float Gravity { get; set; }
        public Vector2 Speed { get; set; } = Vector2.Zero;
        public State CurrentState { get; set; }
        public Vector2 Velocity;
        public RectangleF Bounds;

        public enum State
        {
            Jumping = 0,
            Falling = 1,
            Grounded = 3,
            WallClinging = 4
        };

        public void SetState(List<Abilities.IAbility> abilities)
        {
            var wallJumpIsActive = false;
            var doubleJumpIsActive = false;

            foreach (var a in abilities)
            {
                if (a.GetType() == typeof(Abilities.DoubleJump))
                {
                    doubleJumpIsActive = true;
                }
            }
        }
    }
}
