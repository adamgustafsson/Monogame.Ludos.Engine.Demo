using Microsoft.Xna.Framework;

namespace Monogame.Platformer.Engine.Model
{
    class Actor
    {
        public float Gravity;
        public Vector2 Speed = Vector2.Zero;
        public Vector2 Velocity = Vector2.Zero;
        public Rectangle Bounds;
    }
}
