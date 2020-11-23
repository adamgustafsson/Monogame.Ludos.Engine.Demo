using System.Drawing;

namespace Utillities
{
    static class Utilities
    {
        public static Microsoft.Xna.Framework.Rectangle Round(RectangleF recF)
        {
            var sysRec = Rectangle.Round(recF);
            return new Microsoft.Xna.Framework.Rectangle(sysRec.X, sysRec.Y, sysRec.Width, sysRec.Height);
        }
    }
}
