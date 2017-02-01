using System;
using UnityEngine;

namespace Lerp2API.Optimizers
{
    [Serializable]
    public struct Color : ICloneable
    {
        public object Clone()
        {
            return MemberwiseClone();
        }
        public byte r, g, b, a;
        public static Color white
        {
            get
            {
                return new Color(255, 255, 255);
            }
        }
        public static Color red
        {
            get
            {
                return new Color(255, 0, 0);
            }
        }
        public static Color green
        {
            get
            {
                return new Color(0, 255, 0);
            }
        }
        public static Color blue
        {
            get
            {
                return new Color(0, 0, 255);
            }
        }
        public static Color yellow
        {
            get
            {
                return new Color(255, 255, 0);
            }
        }
        public static Color gray
        {
            get
            {
                return new Color(128, 128, 128);
            }
        }
        public static Color black
        {
            get
            {
                return new Color(0, 0, 0);
            }
        }
        public static Color transparent
        {
            get
            {
                unchecked
                {
                    return new Color(0, 0, 0, 0);
                }
            }
        }
        public Color(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = byte.MaxValue;
        }
        public Color(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        public static bool operator ==(Color c1, Color c2)
        {
            return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a;
        }
        public static bool operator !=(Color c1, Color c2)
        {
            return !(c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            Color c = (Color)obj;
            return r == c.r && g == c.g && b == c.b;
        }
        public static Color operator -(Color c1, Color c2)
        {
            return new Color(
                (byte)Mathf.Clamp(c1.r - c2.r, 0, 255),
                (byte)Mathf.Clamp(c2.g - c2.g, 0, 255),
                (byte)Mathf.Clamp(c2.b - c2.b, 0, 255));
        }
        public static Color operator +(Color c1, Color c2)
        {
            return new Color(
                (byte)Mathf.Clamp(c1.r + c2.r, 0, 255),
                (byte)Mathf.Clamp(c2.g + c2.g, 0, 255),
                (byte)Mathf.Clamp(c2.b + c2.b, 0, 255));
        }
        public Color Lerp(Color c2, float t)
        {
            return new Color(
                (byte)Mathf.Lerp(r, c2.r, t),
                (byte)Mathf.Lerp(g, c2.g, t),
                (byte)Mathf.Lerp(b, c2.b, t));
        }
        public Color Invert()
        {
            return new Color(
                (byte)Mathf.Clamp(byte.MaxValue - r, 0, 255),
                (byte)Mathf.Clamp(byte.MaxValue - g, 0, 255),
                (byte)Mathf.Clamp(byte.MaxValue - b, 0, 255));
        }
        public static explicit operator Color(UnityEngine.Color c)
        {
            return new Color((byte)(c.r * 255), (byte)(c.g * 255), (byte)(c.b * 255));
        }
        public static implicit operator UnityEngine.Color(Color ic)
        {
            return new UnityEngine.Color(ic.r / 255f, ic.g / 255f, ic.b / 255f, ic.a / 255f);
        }
        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", r, g, b, a);
        }
        public static Color[,] Fill(int x, int y)
        {
            Color[,] c = new Color[x, y];
            for (int i = 0; i < x; ++i)
                for (int j = 0; j < y; ++j)
                    c[i, j] = black;
            return c;
        }
    }

}