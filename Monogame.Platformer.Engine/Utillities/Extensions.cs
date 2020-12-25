using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Utillities
{
    static class Extensions
    {
        public static PointF CenterP(this RectangleF rec)
        {
            return new PointF(rec.X + rec.Width / 2,
                              rec.Y + rec.Height / 2);
        }

        public static Vector2 Center(this RectangleF rec)
        {
            return new Vector2(rec.X + rec.Width / 2,
                              rec.Y + rec.Height / 2);
        }

        //public static Vector2 LocationV(this RectangleF rec)
        //{
        //    return new Vector2(rec.X + rec.Width / 2,
        //                      rec.Y + rec.Height / 2);
        //}
    }
}
