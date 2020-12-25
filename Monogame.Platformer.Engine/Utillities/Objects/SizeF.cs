using Microsoft.Xna.Framework;
using System;

namespace Utillities
{
    /// <summary>
    /// Represents a size structure, containing width and height.
    /// 
    /// This object is intended to reproduce the behaviour of System.Drawing.Size and
    /// Sytem.Drawing.SizeF, except use doubles for its internal storage.
    /// </summary>
    public struct SizeF
    {
        #region Private Members
        private float _width;
        private float _height;
        #endregion

        #region Static Members
        /// <summary>
        /// Represents an empty MTPSize object, where the width and height are zero.
        /// </summary>
        public static readonly SizeF Empty;
        #endregion

        #region Constructor(s)
        /// <summary>
        /// Constructs a new object.
        /// </summary>
        /// <param name="width">The desired width.</param>
        /// <param name="height">The desired height.</param>
        public SizeF(float width, float height)
        {
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Constructs a new object.
        /// </summary>
        /// <param name="value">The value that both width and height will be set to.</param>
        public SizeF(float value)
        {
            _width = value;
            _height = value;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The width for this size object.
        /// </summary>
        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        /// <summary>
        /// The height for this object.
        /// </summary>
        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Get a point representing the centre point of this SizeF object.
        /// </summary>
        public Vector2 Centre
        {
            get { return new Vector2(_width / 2f, _height / 2f); }
        }
        #endregion

        #region Operators and Overrides
        /// <summary>
        /// Compare two MTPSize objects for equality.
        /// </summary>
        /// <param name="left">The left hand side of the comparison.</param>
        /// <param name="right">The right hand side of the comparison.</param>
        /// <returns>True if the left and right are equivalent; that is, they have equal values. False otherwise.</returns>
        public static bool operator ==(SizeF left, SizeF right)
        {
            return (left.Width == right.Width && left.Height == right.Height);
        }

        /// <summary>
        /// Compare two MTPSize objects for inequality.
        /// </summary>
        /// <param name="left">The left hand side of the comparison.</param>
        /// <param name="right">The right hand side of the comparison.</param>
        /// <returns>True if the left and right side are not equivalent; that is, they have different values. False otherwise.</returns>
        public static bool operator !=(SizeF left, SizeF right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Creates a string representation of this object.
        /// </summary>
        /// <returns>The string representation of this object.</returns>
        public override string ToString()
        {
            // This is based on the string format of System.Drawing.Size and System.Drawing.SizeF.
            return string.Format("{{Width={0}, Height={1}}}", _width, _height);
        }

        /// <summary>
        /// Checks to see if an object is equal to this object.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>True if the supplied object is equal to this one, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            bool isEqual = false;

            if (obj is SizeF)
            {
                isEqual = ((SizeF)obj) == this;
            }

            return isEqual;
        }

        /// <summary>
        /// Get the hash code for this object.
        /// </summary>
        /// <returns>The hash code for this object.</returns>
        public override int GetHashCode()
        {
            int hash = 17;
            unchecked
            {
                hash = hash * 29 + _width.GetHashCode();
                hash = hash * 29 + _height.GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// Cast a SizeF object to a Vector2 object.
        /// </summary>
        /// <param name="size">The SizeF object to cast.</param>
        /// <returns>The SizeF object as an equivalent Vector2 object.</returns>
        public static implicit operator Vector2(SizeF size)
        {
            return new Vector2(size.Width, size.Height);
        }

        /// <summary>
        /// Cast a Vector2 object to a SizeF object.
        /// </summary>
        /// <param name="vector">The Vector2 object to cast.</param>
        /// <returns>The Vector2 object as an equivalent SizeF object.</returns>
        public static explicit operator SizeF(Vector2 vector)
        {
            return new SizeF(vector.X, vector.Y);
        }

        /// <summary>
        /// Cast a SizeF object to a Point object.
        /// </summary>
        /// <param name="size">The SizeF object to cast.</param>
        /// <returns>The SizeF object as an equivalent Point object.</returns>
        public static explicit operator Point(SizeF size)
        {
            return new Point((int)size.Width, (int)size.Height);
        }

        /// <summary>
        /// Cast a Point object to a SizeF object.
        /// </summary>
        /// <param name="p">The Point object to cast.</param>
        /// <returns>The Point object as an equivalent SizeF object.</returns>
        public static explicit operator SizeF(Point p)
        {
            return new SizeF(p.X, p.Y);
        }

        /// <summary>
        /// Multiply a SizeF object by a float.
        /// </summary>
        /// <param name="lhs">The SizeF object.</param>
        /// <param name="rhs">The float.</param>
        /// <returns>A SizeF object that is the result of the multiplication of the float by both the Width and Height. Effectively a scale operation.</returns>
        public static SizeF operator *(SizeF lhs, float rhs)
        {
            return new SizeF(lhs.Width * rhs, lhs.Height * rhs);
        }

        /// <summary>
        /// Multiply a SizeF object by another SizeF object.
        /// </summary>
        /// <param name="lhs">The first SizeF object.</param>
        /// <param name="rhs">The second SizeF object.</param>
        /// <returns>A new SizeF object where the Width and Height are equal to the product of the supplied objects' Width and Height respectively.</returns>
        public static SizeF operator *(SizeF lhs, SizeF rhs)
        {
            return new SizeF(lhs.Width * rhs.Width, lhs.Height * rhs.Height);
        }

        /// <summary>
        /// Divide a SizeF object by a float.
        /// </summary>
        /// <param name="lhs">The SizeF object.</param>
        /// <param name="rhs">The float.</param>
        /// <returns>A new SizeF object that is the result of the division by the float of the Width and the Height of the SizeF object.</returns>
        public static SizeF operator /(SizeF lhs, float rhs)
        {
            return new SizeF(lhs.Width / rhs, lhs.Height / rhs);
        }

        /// <summary>
        /// Divide a SizeF object by another SizeF object.
        /// </summary>
        /// <param name="lhs">The first SizeF object.</param>
        /// <param name="rhs">The second SizeF object.</param>
        /// <returns>A new SizeF object where the Width and Height are equal to the quotient of the supplied object's Width and Height respectively.</returns>
        public static SizeF operator /(SizeF lhs, SizeF rhs)
        {
            return new SizeF(lhs.Width / rhs.Width, lhs.Height / rhs.Height);
        }

        /// <summary>
        /// Add a SizeF object with another SizeF object.
        /// </summary>
        /// <param name="lhs">The first SizeF object.</param>
        /// <param name="rhs">The second SizeF object.</param>
        /// <returns>A new SizeF object where the Width and Height are equal to the sum of the supplied objects' Width and Height respectively.</returns>
        public static SizeF operator +(SizeF lhs, SizeF rhs)
        {
            return new SizeF(lhs.Width + rhs.Width, lhs.Height + rhs.Height);
        }

        /// <summary>
        /// Sbtract a SizeF object from another SizeF object.
        /// </summary>
        /// <param name="lhs">The first SizeF object.</param>
        /// <param name="rhs">The second SizeF object.</param>
        /// <returns>A new SIzeF object where the Width and Height are equal to the difference of the supplied objects' Width and Height respectively.</returns>
        public static SizeF operator -(SizeF lhs, SizeF rhs)
        {
            return new SizeF(lhs.Width - rhs.Width, lhs.Height - rhs.Height);
        }
        #endregion
    }
}
