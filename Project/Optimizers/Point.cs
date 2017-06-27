using System;
using UnityEngine;

namespace Lerp2API.Optimizers
{
    /// <summary>
    /// Struct Point
    /// </summary>
    [Serializable]
    public struct Point
    {
        /// <summary>
        /// The x
        /// </summary>
        public int x, y;

        /// <summary>
        /// Gets the zero.
        /// </summary>
        /// <value>The zero.</value>
        public static Point zero
        {
            get
            {
                return new Point(0, 0);
            }
        }

        /// <summary>
        /// Gets down.
        /// </summary>
        /// <value>Down.</value>
        public static Point down
        {
            get
            {
                return new Point(0, 1);
            }
        }

        /// <summary>
        /// Gets the left.
        /// </summary>
        /// <value>The left.</value>
        public static Point left
        {
            get
            {
                return new Point(-1, 0);
            }
        }

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>The right.</value>
        public static Point right
        {
            get
            {
                return new Point(1, 0);
            }
        }

        /// <summary>
        /// Gets up.
        /// </summary>
        /// <value>Up.</value>
        public static Point up
        {
            get
            {
                return new Point(0, -1);
            }
        }

        /// <summary>
        /// Gets the upper left.
        /// </summary>
        /// <value>The upper left.</value>
        public static Point upperLeft
        {
            get
            {
                return new Point(-1, -1);
            }
        }

        /// <summary>
        /// Gets the upper right.
        /// </summary>
        /// <value>The upper right.</value>
        public static Point upperRight
        {
            get
            {
                return new Point(1, -1);
            }
        }

        /// <summary>
        /// Gets down left.
        /// </summary>
        /// <value>Down left.</value>
        public static Point downLeft
        {
            get
            {
                return new Point(-1, 1);
            }
        }

        /// <summary>
        /// Gets down right.
        /// </summary>
        /// <value>Down right.</value>
        public static Point downRight
        {
            get
            {
                return new Point(1, 1);
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string name
        {
            get
            {
                return GetName();
            }
        }

        /// <summary>
        /// Gets the SQR magnitude.
        /// </summary>
        /// <value>The SQR magnitude.</value>
        public float sqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }

        /// <summary>
        /// Gets the magnitude.
        /// </summary>
        /// <value>The magnitude.</value>
        public float magnitude
        {
            get
            {
                return Mathf.Sqrt(sqrMagnitude);
            }
        }

        /// <summary>
        /// Gets the normalized.
        /// </summary>
        /// <value>The normalized.</value>
        public Point normalized
        {
            get
            {
                return new Point((int)Mathf.Clamp01(x), (int)Mathf.Clamp01(y));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }

        /// <summary>
        /// Implements the -.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator -(Point a, Point b)
        {
            return new Point(a.x - b.x, a.y - b.y);
        }

        /// <summary>
        /// Implements the *.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator *(Point a, Point b)
        {
            return new Point(a.x * b.x, a.y * b.y);
        }

        /// <summary>
        /// Implements the /.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator /(Point a, Point b)
        {
            return new Point(a.x / b.x, a.y / b.y);
        }

        /// <summary>
        /// Implements the *.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator *(Point a, int b)
        {
            return new Point(a.x * b, a.y * b);
        }

        /// <summary>
        /// Implements the /.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Point operator /(Point a, int b)
        {
            return new Point(a.x / b, a.y / b);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Point"/> to <see cref="Vector2"/>.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Vector2(Point p)
        {
            return new Vector2(p.x, p.y);
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector2"/> to <see cref="Point"/>.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Point(Vector2 v)
        {
            return new Point((int)v.x, (int)v.y);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("({0}, {1})", x, y);
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Point p1, Point p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Point p1, Point p2)
        {
            return p1.x != p2.x || p1.y != p2.y;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            Point p = (Point)obj;
            return x == p.x && y == p.y;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private string GetName()
        {
            if (this == upperLeft)
                return "upperLeft";
            else if (this == up)
                return "up";
            else if (this == upperRight)
                return "upperRight";
            else if (this == left)
                return "left";
            else if (this == right)
                return "right";
            else if (this == downLeft)
                return "downLeft";
            else if (this == down)
                return "down";
            else if (this == downRight)
                return "downRight";
            else if (this == zero)
                return "center";
            else
                return "none";
        }

        /// <summary>
        /// Gets the point.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <returns>Point.</returns>
        public Point GetPoint(Direction dir)
        {
            switch (dir)
            {
                case Direction.up:
                    return up;

                case Direction.upperRight:
                    return upperRight;

                case Direction.right:
                    return right;

                case Direction.downRight:
                    return downRight;

                case Direction.down:
                    return down;

                case Direction.downLeft:
                    return downLeft;

                case Direction.left:
                    return left;

                case Direction.upperLeft:
                    return upperLeft;
            }
            return zero;
        }

        /// <summary>
        /// News the position.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <returns>Point.</returns>
        public Point NewPos(Direction dir)
        {
            return this + GetPoint(dir);
        }
    }

    /// <summary>
    /// Class Dirs.
    /// </summary>
    public class Dirs
    {
        private const int dirLength = 8;

        /// <summary>
        /// Gets the perpendicular.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <returns>Direction.</returns>
        public static Direction GetPerpendicular(Direction dir)
        {
            return GetDir(dir, 2);
        }

        /// <summary>
        /// Gets the clockwise.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <returns>Direction.</returns>
        public static Direction GetClockwise(Direction dir)
        {
            return GetDir(dir, 1);
        }

        /// <summary>
        /// Gets the counter clockwise.
        /// </summary>
        /// <param name="dir">The dir.</param>
        /// <returns>Direction.</returns>
        public static Direction GetCounterClockwise(Direction dir)
        {
            return GetDir(dir, -1);
        }

        private static Direction GetDir(Direction dir, int n)
        {
            int v = (int)dir + n;
            if (v < 0)
                v += dirLength;
            else if (v >= dirLength)
                v -= dirLength;
            return (Direction)v;
        }

        /*private static Direction GetDir(Direction dir, int n)
        {
            if (n == 0) return dir;
            return (int)dir + n < dirLength ? (n >= 0 ? dir : (Direction)dirLength + n) : dir + n - dirLength;
        }*/
    }
}