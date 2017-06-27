using System;
using UnityEngine;

namespace Lerp2API.Optimizers
{
    /// <summary>
    /// Struct Color
    /// </summary>
    /// <seealso cref="System.ICloneable" />
    [Serializable]
    public struct Color : ICloneable
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>System.Object.</returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        /// <summary>
        /// The r
        /// </summary>
        public byte r, g, b, a;

        /// <summary>
        /// Gets the white.
        /// </summary>
        /// <value>The white.</value>
        public static Color white
        {
            get
            {
                return new Color(255, 255, 255);
            }
        }

        /// <summary>
        /// Gets the red.
        /// </summary>
        /// <value>The red.</value>
        public static Color red
        {
            get
            {
                return new Color(255, 0, 0);
            }
        }

        /// <summary>
        /// Gets the green.
        /// </summary>
        /// <value>The green.</value>
        public static Color green
        {
            get
            {
                return new Color(0, 255, 0);
            }
        }

        /// <summary>
        /// Gets the blue.
        /// </summary>
        /// <value>The blue.</value>
        public static Color blue
        {
            get
            {
                return new Color(0, 0, 255);
            }
        }

        /// <summary>
        /// Gets the yellow.
        /// </summary>
        /// <value>The yellow.</value>
        public static Color yellow
        {
            get
            {
                return new Color(255, 255, 0);
            }
        }

        /// <summary>
        /// Gets the gray.
        /// </summary>
        /// <value>The gray.</value>
        public static Color gray
        {
            get
            {
                return new Color(128, 128, 128);
            }
        }

        /// <summary>
        /// Gets the black.
        /// </summary>
        /// <value>The black.</value>
        public static Color black
        {
            get
            {
                return new Color(0, 0, 0);
            }
        }

        /// <summary>
        /// Gets the transparent.
        /// </summary>
        /// <value>The transparent.</value>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        public Color(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = byte.MaxValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <param name="a">a.</param>
        public Color(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Color c1, Color c2)
        {
            return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a;
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Color c1, Color c2)
        {
            return !(c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            Color c = (Color)obj;
            return r == c.r && g == c.g && b == c.b;
        }

        /// <summary>
        /// Implements the -.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static Color operator -(Color c1, Color c2)
        {
            return new Color(
                (byte)Mathf.Clamp(c1.r - c2.r, 0, 255),
                (byte)Mathf.Clamp(c2.g - c2.g, 0, 255),
                (byte)Mathf.Clamp(c2.b - c2.b, 0, 255));
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="c1">The c1.</param>
        /// <param name="c2">The c2.</param>
        /// <returns>The result of the operator.</returns>
        public static Color operator +(Color c1, Color c2)
        {
            return new Color(
                (byte)Mathf.Clamp(c1.r + c2.r, 0, 255),
                (byte)Mathf.Clamp(c2.g + c2.g, 0, 255),
                (byte)Mathf.Clamp(c2.b + c2.b, 0, 255));
        }

        /// <summary>
        /// Lerps the specified c2.
        /// </summary>
        /// <param name="c2">The c2.</param>
        /// <param name="t">The t.</param>
        /// <returns>Color.</returns>
        public Color Lerp(Color c2, float t)
        {
            return new Color(
                (byte)Mathf.Lerp(r, c2.r, t),
                (byte)Mathf.Lerp(g, c2.g, t),
                (byte)Mathf.Lerp(b, c2.b, t));
        }

        /// <summary>
        /// Inverts this instance.
        /// </summary>
        /// <returns>Color.</returns>
        public Color Invert()
        {
            return new Color(
                (byte)Mathf.Clamp(byte.MaxValue - r, 0, 255),
                (byte)Mathf.Clamp(byte.MaxValue - g, 0, 255),
                (byte)Mathf.Clamp(byte.MaxValue - b, 0, 255));
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="UnityEngine.Color"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Color(UnityEngine.Color c)
        {
            return new Color((byte)(c.r * 255), (byte)(c.g * 255), (byte)(c.b * 255));
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Color"/> to <see cref="UnityEngine.Color"/>.
        /// </summary>
        /// <param name="ic">The ic.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator UnityEngine.Color(Color ic)
        {
            return new UnityEngine.Color(ic.r / 255f, ic.g / 255f, ic.b / 255f, ic.a / 255f);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("({0}, {1}, {2}, {3})", r, g, b, a);
        }

        /// <summary>
        /// Fills the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>Color[].</returns>
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