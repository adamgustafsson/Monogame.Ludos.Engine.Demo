using Microsoft.Xna.Framework;
using Model.Actors.Abilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model.Actors
{
    public class Actor
    {
        public float Gravity { get; set; }
        public Vector2 Speed { get; set; } = Vector2.Zero;
        public State CurrentState { get; set; }
        public bool OnLadder { get; set; }
        public List<IAbility> Abilities { get; set; } = new List<IAbility>();

        public Vector2 Velocity;
        public RectangleF Bounds;

        public enum State
        {
            Jumping = 0,
            Falling = 1,
            Idle = 3,
            WallClinging = 4,
            MovingRight = 5,
            MovingLeft = 6,
            ClimbingUp = 7,
            ClimbingDown = 8,
            ClimbingIdle = 9
        };

        public void SetState()
        {
            if (Velocity.Y < 0 && OnLadder)
                CurrentState = State.ClimbingUp;
            else if (Velocity.Y > 0 && OnLadder)
                CurrentState = State.ClimbingDown;
            else if (OnLadder)
                CurrentState = State.ClimbingIdle;
            else if (Velocity.Y < 0)
                CurrentState = State.Jumping;
            else if (GetAbility<WallJump>()?.IsWallClinging ?? false)
                CurrentState = State.WallClinging;
            else if (Velocity.Y > 0)
                CurrentState = State.Falling;
            else if (Velocity.X > 0)
                CurrentState = State.MovingRight;
            else if (Velocity.X < 0)
                CurrentState = State.MovingLeft;
            else
                CurrentState = State.Idle;
        }

        public T GetAbility<T>()
        {
            return (T)Abilities.Where(x => x.GetType() == typeof(T) && x.AbilityEnabled == true).FirstOrDefault();
        }
    }
}
