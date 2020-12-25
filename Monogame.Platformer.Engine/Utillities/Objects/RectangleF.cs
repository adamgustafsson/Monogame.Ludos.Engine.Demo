//using Microsoft.Xna.Framework;
//using System;

//namespace Utillities
//{ 
//    /// <summary>
//    /// Represents a rectangle structure, having both a location, as well a width and height.
//    /// The width and height of a rectangle are relative to the location; therefore, they can
//    /// be negative.
//    /// 
//    /// This object is intended to reproduce the behaviour of System.Drawing.Rectangle and
//    /// Sytem.Drawing.RectangleF, except use doubles for its internal storage.
//    /// </summary>
//    public struct RectangleF
//    {
//        #region Private Members
//        private Vector2 _location;
//        private SizeF _size;
//        #endregion

//        #region Constructor(s)
//        /// <summary>
//        /// Create a new rectangle object.
//        /// </summary>
//        /// <param name="location">The location of this rectangle.</param>
//        /// <param name="size">The size of this rectangle.</param>
//        public RectangleF(Vector2 location, SizeF size)
//        {
//            _location = location;
//            _size = size;
//        }

//        /// <summary>
//        /// Create a new rectangle object.
//        /// </summary>
//        /// <param name="p1">The first corner of this rectangle.</param>
//        /// <param name="p2">The second corner of this rectangle.</param>
//        public RectangleF(Vector2 p1, Vector2 p2)
//            : this(p1, new SizeF(p2.X - p1.X, p2.Y - p1.Y))
//        {
//        }

//        /// <summary>
//        /// Create a new rectangle object.
//        /// </summary>
//        /// <param name="x">The X coordinate of the location.</param>
//        /// <param name="y">The Y coordinate of the location.</param>
//        /// <param name="width">The width of this rectangle.</param>
//        /// <param name="height">The height of this rectangle.</param>
//        public RectangleF(float x, float y, float width, float height)
//            : this(new Vector2(x, y), new SizeF(width, height))
//        {
//        }
//        #endregion

//        #region Static Members
//        /// <summary>
//        /// Represents an empty rectangle, where the location and size are initialized to zero.
//        /// </summary>
//        public static readonly RectangleF Empty;
//        #endregion

//        #region Public Properties
//        /// <summary>
//        /// The location of this rectangle.
//        /// </summary>
//        public Vector2 Location
//        {
//            get { return _location; }
//            set { _location = value; }
//        }

//        /// <summary>
//        /// The size of this rectangle.
//        /// </summary>
//        public SizeF Size
//        {
//            get { return _size; }
//            set { _size = value; }
//        }

//        /// <summary>
//        /// The X coordinate of this rectangle's location.
//        /// </summary>
//        public float X
//        {
//            get { return _location.X; }
//            set { _location.X = value; }
//        }

//        /// <summary>
//        /// The Y coordinate of this rectangle's location.
//        /// </summary>
//        public float Y
//        {
//            get { return _location.Y; }
//            set { _location.Y = value; }
//        }

//        /// <summary>
//        /// The width of this rectangle.
//        /// </summary>
//        public float Width
//        {
//            get { return _size.Width; }
//            set { _size.Width = value; }
//        }

//        /// <summary>
//        /// The height of this rectangle.
//        /// </summary>
//        public float Height
//        {
//            get { return _size.Height; }
//            set { _size.Height = value; }
//        }

//        /// <summary>
//        /// The location of this rectangle's opposite corner (relative to its location).
//        /// </summary>
//        public Vector2 OppositeCorner
//        {
//            get { return new Vector2(_location.X + _size.Width, _location.Y + _size.Height); }
//        }

//        /// <summary>
//        /// The Y coordinate for this rectangle's top edge.
//        /// </summary>
//        public float Top { get { return this.Y; } }

//        /// <summary>
//        /// The X coordinate for this rectangle's left edge.
//        /// </summary>
//        public float Left { get { return this.X; } }

//        /// <summary>
//        /// The X coordinate for this rectangle's right edge.
//        /// </summary>
//        public float Right { get { return this.X + this.Width; } }

//        /// <summary>
//        /// The Y coordinate for this rectangle's bottom edge.
//        /// </summary>
//        public float Bottom { get { return this.Y + this.Height; } }

//        /// <summary>
//        /// The centre point of this rectangle.
//        /// </summary>
//        public Vector2 Centre
//        {
//            get
//            {
//                return new Vector2(this.Left + this.Width / 2f, this.Top + this.Height / 2f);
//            }
//        }
//        #endregion

//        #region Operators and Overloads
//        /// <summary>
//        /// Compare two rectangles for equality.
//        /// </summary>
//        /// <param name="left">The left rectangle.</param>
//        /// <param name="right">The right rectangle.</param>
//        /// <returns>True if both rectangles have the same location and size, false otherwise.</returns>
//        public static bool operator ==(RectangleF left, RectangleF right)
//        {
//            Vector2 aMin = new Vector2(Math.Min(left.Left, left.Right), Math.Min(left.Top, left.Bottom));
//            Vector2 aMax = new Vector2(Math.Max(left.Left, left.Right), Math.Max(left.Top, left.Bottom));

//            Vector2 bMin = new Vector2(Math.Min(right.Left, right.Right), Math.Min(right.Top, right.Bottom));
//            Vector2 bMax = new Vector2(Math.Max(right.Left, right.Right), Math.Max(right.Top, right.Bottom));

//            return (aMin == bMin && aMax == bMax);
//        }

//        /// <summary>
//        /// Compare two rectangles for inequality.
//        /// </summary>
//        /// <param name="left">The left rectangle.</param>
//        /// <param name="right">The right rectangle.</param>
//        /// <returns>True if both rectangles have different locations or sizes, false otherwise.</returns>
//        public static bool operator !=(RectangleF left, RectangleF right)
//        {
//            return !(left == right);
//        }

//        /// <summary>
//        /// Creates a string representation of this object.
//        /// </summary>
//        /// <returns>The string representation of this object.</returns>
//        public override string ToString()
//        {
//            // This is based on the string format of System.Drawing.Rectangle and System.Drawing.RectangleF.
//            return string.Format("{{X={0}, Y={1}, Width={2}, Height={3}}}", this.X, this.Y, this.Width, this.Height);
//        }

//        /// <summary>
//        /// Checks to see if an object is equal to this object.
//        /// </summary>
//        /// <param name="obj">The object to test.</param>
//        /// <returns>True if the supplied object is equal to this one, false otherwise.</returns>
//        public override bool Equals(object obj)
//        {
//            bool isEqual = false;

//            if (obj is RectangleF)
//            {
//                isEqual = ((RectangleF)obj) == this;
//            }

//            return isEqual;
//        }

//        /// <summary>
//        /// Get the hash code for this object.
//        /// </summary>
//        /// <returns>The hash code for this object.</returns>
//        public override int GetHashCode()
//        {
//            int hash = 17;
//            unchecked
//            {
//                hash = hash * 29 + _location.GetHashCode();
//                hash = hash * 29 + _size.GetHashCode();
//            }

//            return hash;
//        }
//        #endregion

//        #region Public Methods
//        /// <summary>
//        /// Adjust the edges of this rectangle by the specified amounts.
//        /// </summary>
//        /// <param name="horizontalAmount">The amount to adjust the left and right edges.</param>
//        /// <param name="verticalAmount">The amount to adjust the top and buttom edges.</param>
//        public void Inflate(float horizontalAmount, float verticalAmount)
//        {
//            this.X -= horizontalAmount;
//            this.Width += horizontalAmount * 2; ;

//            this.Y -= verticalAmount;
//            this.Height += verticalAmount * 2;
//        }

//        /// <summary>
//        /// Tests to see if a point is contained within this rectangle.
//        /// 
//        /// NOTE: This method deviates from the System.Drawing equivalents. This method will work
//        ///       with return the correct result when width and height are negative.
//        /// </summary>
//        /// <param name="point">The point to test.</param>
//        /// <returns>True if the point is inside the rectangle, false otherwise.</returns>
//        public bool Contains(Vector2 point)
//        {
//            Vector2 min = new Vector2(Math.Min(this.Left, this.Right), Math.Min(this.Top, this.Bottom));
//            Vector2 max = new Vector2(Math.Max(this.Left, this.Right), Math.Max(this.Top, this.Bottom));

//            return (point.X >= min.X && point.X < max.X && point.Y >= min.Y && point.Y < max.Y);
//        }

//        /// <summary>
//        /// Tests to see if a point is contained within this rectangle.
//        /// </summary>
//        /// <param name="x">The X coordinate of the point.</param>
//        /// <param name="y">The Y coordinate of the point.</param>
//        /// <returns>True if the point is inside the rectangle, false otherwise.</returns>
//        public bool Contains(float x, float y)
//        {
//            return this.Contains(new Vector2(x, y));
//        }

//        /// <summary>
//        /// Tests to see if a rectangle is contained within this rectangle.
//        /// 
//        /// NOTE: This method deviates from the System.Drawing equivalents. This method will work
//        ///       with return the correct result when width and height are negative.
//        /// </summary>
//        /// <param name="rect">The rectangle to test.</param>
//        /// <returns>True if the supplied rectangle is contained by this object, false otherwise.</returns>
//        public bool Contains(RectangleF rect)
//        {
//            Vector2 thisMin = new Vector2(Math.Min(this.Left, this.Right), Math.Min(this.Top, this.Bottom));
//            Vector2 thisMax = new Vector2(Math.Max(this.Left, this.Right), Math.Max(this.Top, this.Bottom));

//            Vector2 rectMin = new Vector2(Math.Min(rect.Left, rect.Right), Math.Min(rect.Top, rect.Bottom));
//            Vector2 rectMax = new Vector2(Math.Max(rect.Left, rect.Right), Math.Max(rect.Top, rect.Bottom));

//            return (rectMin.X >= thisMin.X && rectMax.X <= thisMax.X && rectMin.Y >= thisMin.Y && rectMax.Y <= thisMax.Y);
//        }

//        /// <summary>
//        /// Tests to see if a rectangle intersects with this rectangle.
//        /// 
//        /// NOTE: This method deviates from the System.Drawing equivalents. This method will work
//        ///       with return the correct result when width and height are negative.
//        /// </summary>
//        /// <param name="rect">The rectangle to test.</param>
//        /// <returns>True if this object and the supplied rectangle intersect, false otherwise.</returns>
//        public bool IntersectsWith(RectangleF rect)
//        {
//            Vector2 thisMin = new Vector2(Math.Min(this.Left, this.Right), Math.Min(this.Top, this.Bottom));
//            Vector2 thisMax = new Vector2(Math.Max(this.Left, this.Right), Math.Max(this.Top, this.Bottom));

//            Vector2 rectMin = new Vector2(Math.Min(rect.Left, rect.Right), Math.Min(rect.Top, rect.Bottom));
//            Vector2 rectMax = new Vector2(Math.Max(rect.Left, rect.Right), Math.Max(rect.Top, rect.Bottom));

//            return !((rectMax.X <= thisMin.X || rectMin.X >= thisMax.X) || (rectMin.Y >= thisMax.Y || rectMax.Y <= thisMin.Y));
//        }


//        /// <summary>
//        /// Generate a new rectangle that is the intersection of two rectangles; that is, the portion
//        /// common to both rectangles (overlap).
//        /// 
//        /// NOTE: This method deviates from the System.Drawing equivalents. This method will work
//        ///       with return the correct result when width and height values are negative.
//        /// </summary>
//        /// <param name="a">The first rectangle.</param>
//        /// <param name="b">The second rectangle.</param>
//        /// <returns>A rectangle representing the common area of the two rectangles.</returns>
//        public static RectangleF Intersect(RectangleF a, RectangleF b)
//        {
//            RectangleF result = RectangleF.Empty;

//            if (a.IntersectsWith(b))
//            {
//                Vector2 aMin = new Vector2(Math.Min(a.Left, a.Right), Math.Min(a.Top, a.Bottom));
//                Vector2 aMax = new Vector2(Math.Max(a.Left, a.Right), Math.Max(a.Top, a.Bottom));

//                Vector2 bMin = new Vector2(Math.Min(b.Left, b.Right), Math.Min(b.Top, b.Bottom));
//                Vector2 bMax = new Vector2(Math.Max(b.Left, b.Right), Math.Max(b.Top, b.Bottom));

//                result.X = Math.Max(aMin.X, bMin.X);
//                result.Y = Math.Max(aMin.Y, bMin.Y);
//                result.Width = Math.Min(aMax.X, bMax.X) - result.X;
//                result.Height = Math.Min(aMax.Y, bMax.Y) - result.Y;
//            }

//            return result;
//        }

//        /// <summary>
//        /// Generate a new rectangle that is the union of two rectangles; that is, the
//        /// rectangle that would minimally contain both supplied rectangles.
//        /// 
//        /// NOTE: This method deviates from the System.Drawing equivalents. This method will work
//        ///       with return the correct result when width and height values are negative.
//        /// </summary>
//        /// <param name="a">The first rectangle.</param>
//        /// <param name="b">The second rectangle.</param>
//        /// <returns>A rectangle representing the union of the two rectangles.</returns>
//        public static RectangleF Union(RectangleF a, RectangleF b)
//        {
//            RectangleF result = RectangleF.Empty;

//            Vector2 aMin = new Vector2(Math.Min(a.Left, a.Right), Math.Min(a.Top, a.Bottom));
//            Vector2 aMax = new Vector2(Math.Max(a.Left, a.Right), Math.Max(a.Top, a.Bottom));

//            Vector2 bMin = new Vector2(Math.Min(b.Left, b.Right), Math.Min(b.Top, b.Bottom));
//            Vector2 bMax = new Vector2(Math.Max(b.Left, b.Right), Math.Max(b.Top, b.Bottom));

//            result.X = Math.Min(aMin.X, bMin.X);
//            result.Y = Math.Min(aMin.Y, bMin.Y);
//            result.Width = Math.Max(aMax.X, bMax.X) - result.X;
//            result.Height = Math.Max(aMax.Y, bMax.Y) - result.Y;

//            return result;
//        }
//        #endregion

//        #region Casting
//        /// <summary>
//        /// Cast a RectangleF object to a Rectangle object.
//        /// </summary>
//        /// <param name="rect">The RectangleF object to cast.</param>
//        /// <returns>A Rectangle object which is equivalent to the supplied RectangleF object.</returns>
//        public static explicit operator Rectangle(RectangleF rect)
//        {
//            return new Rectangle((int)Math.Floor(rect.X), (int)Math.Floor(rect.Y), (int)Math.Floor(rect.Width), (int)Math.Floor(rect.Height));
//        }

//        /// <summary>
//        /// Cast a Rectangle object to a RectangleF object.
//        /// </summary>
//        /// <param name="rect">The Rectangle object to cast.</param>
//        /// <returns>A RectangleF object which is equivalent to the supplied Rectangle object.</returns>
//        public static explicit operator RectangleF(Rectangle rect)
//        {
//            return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
//        }
//        #endregion
//    }
//}
