// Type: UnityEngine.Vector2
// Assembly: UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Program Files (x86)\Unity\Editor\Data\Managed\UnityEngine.dll
using System;

namespace UnityEngine
{
    /// <summary>
    /// Struct Vector2d
    /// </summary>
    public struct Vector2d
    {
        /// <summary>
        /// The k epsilon
        /// </summary>
        public const double kEpsilon = 1E-05d;

        /// <summary>
        /// The x
        /// </summary>
        public double x;

        /// <summary>
        /// The y
        /// </summary>
        public double y;

        /// <summary>
        /// Gets or sets the <see cref="System.Double"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Invalid Vector2d index!
        /// or
        /// Invalid Vector2d index!
        /// </exception>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.x;

                    case 1:
                        return this.y;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2d index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.x = value;
                        break;

                    case 1:
                        this.y = value;
                        break;

                    default:
                        throw new IndexOutOfRangeException("Invalid Vector2d index!");
                }
            }
        }

        /// <summary>
        /// Gets the normalized.
        /// </summary>
        /// <value>The normalized.</value>
        public Vector2d normalized
        {
            get
            {
                Vector2d vector2d = new Vector2d(this.x, this.y);
                vector2d.Normalize();
                return vector2d;
            }
        }

        /// <summary>
        /// Gets the magnitude.
        /// </summary>
        /// <value>The magnitude.</value>
        public double magnitude
        {
            get
            {
                return Mathd.Sqrt(this.x * this.x + this.y * this.y);
            }
        }

        /// <summary>
        /// Gets the SQR magnitude.
        /// </summary>
        /// <value>The SQR magnitude.</value>
        public double sqrMagnitude
        {
            get
            {
                return this.x * this.x + this.y * this.y;
            }
        }

        /// <summary>
        /// Gets the zero.
        /// </summary>
        /// <value>The zero.</value>
        public static Vector2d zero
        {
            get
            {
                return new Vector2d(0.0d, 0.0d);
            }
        }

        /// <summary>
        /// Gets the one.
        /// </summary>
        /// <value>The one.</value>
        public static Vector2d one
        {
            get
            {
                return new Vector2d(1d, 1d);
            }
        }

        /// <summary>
        /// Gets up.
        /// </summary>
        /// <value>Up.</value>
        public static Vector2d up
        {
            get
            {
                return new Vector2d(0.0d, 1d);
            }
        }

        /// <summary>
        /// Gets the right.
        /// </summary>
        /// <value>The right.</value>
        public static Vector2d right
        {
            get
            {
                return new Vector2d(1d, 0.0d);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2d"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public Vector2d(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Vector3d"/> to <see cref="Vector2d"/>.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Vector2d(Vector3d v)
        {
            return new Vector2d(v.x, v.y);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Vector2d"/> to <see cref="Vector3d"/>.
        /// </summary>
        /// <param name="v">The v.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Vector3d(Vector2d v)
        {
            return new Vector3d(v.x, v.y, 0.0d);
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2d operator +(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.x + b.x, a.y + b.y);
        }

        /// <summary>
        /// Implements the -.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2d operator -(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.x - b.x, a.y - b.y);
        }

        /// <summary>
        /// Implements the -.
        /// </summary>
        /// <param name="a">a.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2d operator -(Vector2d a)
        {
            return new Vector2d(-a.x, -a.y);
        }

        /// <summary>
        /// Implements the *.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="d">The d.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2d operator *(Vector2d a, double d)
        {
            return new Vector2d(a.x * d, a.y * d);
        }

        /// <summary>
        /// Implements the *.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="a">a.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2d operator *(float d, Vector2d a)
        {
            return new Vector2d(a.x * d, a.y * d);
        }

        /// <summary>
        /// Implements the /.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="d">The d.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2d operator /(Vector2d a, double d)
        {
            return new Vector2d(a.x / d, a.y / d);
        }

        /// <summary>
        /// Implements the ==.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(Vector2d lhs, Vector2d rhs)
        {
            return Vector2d.SqrMagnitude(lhs - rhs) < 0.0 / 1.0;
        }

        /// <summary>
        /// Implements the !=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(Vector2d lhs, Vector2d rhs)
        {
            return (double)Vector2d.SqrMagnitude(lhs - rhs) >= 0.0 / 1.0;
        }

        /// <summary>
        /// Sets the specified new x.
        /// </summary>
        /// <param name="new_x">The new x.</param>
        /// <param name="new_y">The new y.</param>
        public void Set(double new_x, double new_y)
        {
            this.x = new_x;
            this.y = new_y;
        }

        /// <summary>
        /// Lerps the specified from.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="t">The t.</param>
        /// <returns>Vector2d.</returns>
        public static Vector2d Lerp(Vector2d from, Vector2d to, double t)
        {
            t = Mathd.Clamp01(t);
            return new Vector2d(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t);
        }

        /// <summary>
        /// Moves the towards.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="target">The target.</param>
        /// <param name="maxDistanceDelta">The maximum distance delta.</param>
        /// <returns>Vector2d.</returns>
        public static Vector2d MoveTowards(Vector2d current, Vector2d target, double maxDistanceDelta)
        {
            Vector2d vector2 = target - current;
            double magnitude = vector2.magnitude;
            if (magnitude <= maxDistanceDelta || magnitude == 0.0d)
                return target;
            else
                return current + vector2 / magnitude * maxDistanceDelta;
        }

        /// <summary>
        /// Scales the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>Vector2d.</returns>
        public static Vector2d Scale(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.x * b.x, a.y * b.y);
        }

        /// <summary>
        /// Scales the specified scale.
        /// </summary>
        /// <param name="scale">The scale.</param>
        public void Scale(Vector2d scale)
        {
            this.x *= scale.x;
            this.y *= scale.y;
        }

        /// <summary>
        /// Normalizes this instance.
        /// </summary>
        public void Normalize()
        {
            double magnitude = this.magnitude;
            if (magnitude > 9.99999974737875E-06)
                this = this / magnitude;
            else
                this = Vector2d.zero;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            /*
      string fmt = "({0:D1}, {1:D1})";
      object[] objArray = new object[2];
      int index1 = 0;
      // ISSUE: variable of a boxed type
      __Boxed<double> local1 = (ValueType) this.x;
      objArray[index1] = (object) local1;
      int index2 = 1;
      // ISSUE: variable of a boxed type
      __Boxed<double> local2 = (ValueType) this.y;
      objArray[index2] = (object) local2;
      */
            return "not implemented";
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public string ToString(string format)
        {
            /* TODO:
      string fmt = "({0}, {1})";
      object[] objArray = new object[2];
      int index1 = 0;
      string str1 = this.x.ToString(format);
      objArray[index1] = (object) str1;
      int index2 = 1;
      string str2 = this.y.ToString(format);
      objArray[index2] = (object) str2;
      */
            return "not implemented";
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object other)
        {
            if (!(other is Vector2d))
                return false;
            Vector2d vector2d = (Vector2d)other;
            if (this.x.Equals(vector2d.x))
                return this.y.Equals(vector2d.y);
            else
                return false;
        }

        /// <summary>
        /// Dots the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>System.Double.</returns>
        public static double Dot(Vector2d lhs, Vector2d rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        /// <summary>
        /// Angles the specified from.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns>System.Double.</returns>
        public static double Angle(Vector2d from, Vector2d to)
        {
            return Mathd.Acos(Mathd.Clamp(Vector2d.Dot(from.normalized, to.normalized), -1d, 1d)) * 57.29578d;
        }

        /// <summary>
        /// Distances the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Double.</returns>
        public static double Distance(Vector2d a, Vector2d b)
        {
            return (a - b).magnitude;
        }

        /// <summary>
        /// Clamps the magnitude.
        /// </summary>
        /// <param name="vector">The vector.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns>Vector2d.</returns>
        public static Vector2d ClampMagnitude(Vector2d vector, double maxLength)
        {
            if (vector.sqrMagnitude > maxLength * maxLength)
                return vector.normalized * maxLength;
            else
                return vector;
        }

        /// <summary>
        /// SQRs the magnitude.
        /// </summary>
        /// <param name="a">a.</param>
        /// <returns>System.Double.</returns>
        public static double SqrMagnitude(Vector2d a)
        {
            return (a.x * a.x + a.y * a.y);
        }

        /// <summary>
        /// SQRs the magnitude.
        /// </summary>
        /// <returns>System.Double.</returns>
        public double SqrMagnitude()
        {
            return (this.x * this.x + this.y * this.y);
        }

        /// <summary>
        /// Minimums the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>Vector2d.</returns>
        public static Vector2d Min(Vector2d lhs, Vector2d rhs)
        {
            return new Vector2d(Mathd.Min(lhs.x, rhs.x), Mathd.Min(lhs.y, rhs.y));
        }

        /// <summary>
        /// Maximums the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>Vector2d.</returns>
        public static Vector2d Max(Vector2d lhs, Vector2d rhs)
        {
            return new Vector2d(Mathd.Max(lhs.x, rhs.x), Mathd.Max(lhs.y, rhs.y));
        }
    }
}