using Microsoft.Xna.Framework;
using System.Drawing;

namespace Model
{
    public class Actor
    {
        public float Gravity { get; set; }
        public Vector2 Speed { get; set; } = Vector2.Zero;
        public Vector2 Velocity;
        public RectangleF Bounds;
    }
}
