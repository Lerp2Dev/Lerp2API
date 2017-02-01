using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Lerp2API.Optimizers
{
    [Serializable]
    public struct Point
    {
        public int x, y;
        public static Point zero
        {
            get
            {
                return new Point(0, 0);
            }
        }
        public static Point down
        {
            get
            {
                return new Point(0, 1);
            }
        }
        public static Point left
        {
            get
            {
                return new Point(-1, 0);
            }
        }
        public static Point right
        {
            get
            {
                return new Point(1, 0);
            }
        }
        public static Point up
        {
            get
            {
                return new Point(0, -1);
            }
        }
        public static Point upperLeft
        {
            get
            {
                return new Point(-1, -1);
            }
        }
        public static Point upperRight
        {
            get
            {
                return new Point(1, -1);
            }
        }
        public static Point downLeft
        {
            get
            {
                return new Point(-1, 1);
            }
        }
        public static Point downRight
        {
            get
            {
                return new Point(1, 1);
            }
        }
        public string name
        {
            get
            {
                return GetName();
            }
        }
        public float sqrMagnitude
        {
            get
            {
                return x * x + y * y;
            }
        }
        public float magnitude
        {
            get
            {
                return Mathf.Sqrt(sqrMagnitude);
            }
        }
        public Point normalized
        {
            get
            {
                return new Point((int)Mathf.Clamp01(x), (int)Mathf.Clamp01(y));
            }
        }
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }
        public static Point operator -(Point a, Point b)
        {
            return new Point(a.x - b.x, a.y - b.y);
        }
        public static Point operator *(Point a, Point b)
        {
            return new Point(a.x * b.x, a.y * b.y);
        }
        public static Point operator /(Point a, Point b)
        {
            return new Point(a.x / b.x, a.y / b.y);
        }
        public static Point operator *(Point a, int b)
        {
            return new Point(a.x * b, a.y * b);
        }
        public static Point operator /(Point a, int b)
        {
            return new Point(a.x / b, a.y / b);
        }
        public static implicit operator Vector2(Point p)
        {
            return new Vector2(p.x, p.y);
        }
        public static explicit operator Point(Vector2 v)
        {
            return new Point((int)v.x, (int)v.y);
        }
        public override string ToString()
        {
            return string.Format("({0}, {1})", x, y);
        }
        public static bool operator ==(Point p1, Point p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }
        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1.x == p2.x && p1.y == p2.y);
        }
        public override bool Equals(object obj)
        {
            Point p = (Point)obj;
            return x == p.x && y == p.y;
        }
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
        public Point NewPos(Direction dir)
        {
            return this + GetPoint(dir);
        }

    }

    public enum Direction { up, upperRight, right, downRight, down, downLeft, left, upperLeft }
    public class Dirs
    {
        private const int dirLength = 8;
        public static Direction GetPerpendicular(Direction dir)
        {
            return GetDir(dir, 2);
        }
        public static Direction GetClockwise(Direction dir)
        {
            return GetDir(dir, 1);
        }
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