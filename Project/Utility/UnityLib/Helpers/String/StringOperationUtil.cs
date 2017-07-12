using System.Threading;
using System.Text;

// Info: OptimizedStringOperation
// Concatenating many strings allocates a lot of temporary memory in the managed heap. It is recommend to use System.Text.StringBuffer instead.
// Sometimes this is hard to fix, especially if you already have written a lot of code. This solution should help you to work around that problem easily.
// source: https://bitbucket.org/Unity-Technologies/enterprise-support

namespace StringOperationUtil
{
    /// <summary>
    /// Using this,you can optimize string concat operation easily.
    /// To use this , you should put this on the top of code.
    /// ------
    /// using StrOpe = StringOperationUtil.OptimizedStringOperation;
    /// ------
    ///
    /// - before code
    /// string str = "aaa" + 20 + "bbbb";
    ///
    /// - after code
    /// string str = StrOpe.i + "aaa" + 20 + "bbbb";
    ///
    /// "StrOpe.i" is for MainThread , do not call from other theads.
    /// If "StrOpe.i" is called from Mainthread , reuse same object.
    ///
    /// You can also use "StrOpe.small" / "StrOpe.medium" / "StrOpe.large" instead of "StrOpe.i".
    /// These are creating instance.
    /// </summary>
    public class OptimizedStringOperation
    {
        private static OptimizedStringOperation instance = null;
#if !UNITY_WEBGL
        private static Thread singletonThread = null;
#endif
        private StringBuilder sb = null;

        static OptimizedStringOperation()
        {
            instance = new OptimizedStringOperation(1024);
        }

        private OptimizedStringOperation(int capacity)
        {
            sb = new StringBuilder(capacity);
        }

        /// <summary>
        /// Creates the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <returns>OptimizedStringOperation.</returns>
        public static OptimizedStringOperation Create(int capacity)
        {
            return new OptimizedStringOperation(capacity);
        }

        /// <summary>
        /// Gets the small.
        /// </summary>
        /// <value>The small.</value>
        public static OptimizedStringOperation small
        {
            get
            {
                return Create(64);
            }
        }

        /// <summary>
        /// Gets the medium.
        /// </summary>
        /// <value>The medium.</value>
        public static OptimizedStringOperation medium
        {
            get
            {
                return Create(256);
            }
        }

        /// <summary>
        /// Gets the large.
        /// </summary>
        /// <value>The large.</value>
        public static OptimizedStringOperation large
        {
            get
            {
                return Create(1024);
            }
        }

        /// <summary>
        /// Gets the i.
        /// </summary>
        /// <value>The i.</value>
        public static OptimizedStringOperation i
        {
            get
            {
#if !UNITY_WEBGL
                // Bind instance to thread.
                if (singletonThread == null)
                {
                    singletonThread = Thread.CurrentThread;
                }
                // check thread...
                if (singletonThread != Thread.CurrentThread)
                {
#if DEBUG || UNITY_EDITOR
                    UnityEngine.Debug.LogError("Execute from another thread.");
#endif
                    return small;
                }
#endif
                instance.sb.Length = 0;
                return instance;
            }
        }

        /// <summary>
        /// Gets or sets the capacity.
        /// </summary>
        /// <value>The capacity.</value>
        public int Capacity
        {
            set { this.sb.Capacity = value; }
            get { return sb.Capacity; }
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            set { this.sb.Length = value; }
            get { return this.sb.Length; }
        }

        /// <summary>
        /// Removes the specified start index.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="length">The length.</param>
        /// <returns>OptimizedStringOperation.</returns>
        public OptimizedStringOperation Remove(int startIndex, int length)
        {
            sb.Remove(startIndex, length);
            return this;
        }

        /// <summary>
        /// Replaces the specified old value.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>OptimizedStringOperation.</returns>
        public OptimizedStringOperation Replace(string oldValue, string newValue)
        {
            sb.Replace(oldValue, newValue);
            return this;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return sb.ToString();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            // StringBuilder.Clear() doesn't support .Net 3.5...
            // "Capasity = 0" doesn't work....
            sb = new StringBuilder(0);
        }

        /// <summary>
        /// To the lower.
        /// </summary>
        /// <returns>OptimizedStringOperation.</returns>
        public OptimizedStringOperation ToLower()
        {
            int length = sb.Length;
            for (int i = 0; i < length; ++i)
            {
                if (char.IsUpper(sb[i]))
                {
                    sb.Replace(sb[i], char.ToLower(sb[i]), i, 1);
                }
            }
            return this;
        }

        /// <summary>
        /// To the upper.
        /// </summary>
        /// <returns>OptimizedStringOperation.</returns>
        public OptimizedStringOperation ToUpper()
        {
            int length = sb.Length;
            for (int i = 0; i < length; ++i)
            {
                if (char.IsLower(sb[i]))
                {
                    sb.Replace(sb[i], char.ToUpper(sb[i]), i, 1);
                }
            }
            return this;
        }

        /// <summary>
        /// Trims this instance.
        /// </summary>
        /// <returns>OptimizedStringOperation.</returns>
        public OptimizedStringOperation Trim()
        {
            return TrimEnd().TrimStart();
        }

        /// <summary>
        /// Trims the start.
        /// </summary>
        /// <returns>OptimizedStringOperation.</returns>
        public OptimizedStringOperation TrimStart()
        {
            int length = sb.Length;
            for (int i = 0; i < length; ++i)
            {
                if (!char.IsWhiteSpace(sb[i]))
                {
                    if (i > 0)
                    {
                        sb.Remove(0, i);
                    }
                    break;
                }
            }
            return this;
        }

        /// <summary>
        /// Trims the end.
        /// </summary>
        /// <returns>OptimizedStringOperation.</returns>
        public OptimizedStringOperation TrimEnd()
        {
            int length = sb.Length;
            for (int i = length - 1; i >= 0; --i)
            {
                if (!char.IsWhiteSpace(sb[i]))
                {
                    if (i < length - 1)
                    {
                        sb.Remove(i, length - i);
                    }
                    break;
                }
            }
            return this;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="OptimizedStringOperation"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(OptimizedStringOperation t)
        {
            return t.ToString();
        }

        #region ADD_OPERATOR

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="v">if set to <c>true</c> [v].</param>
        /// <returns>The result of the operator.</returns>
        public static OptimizedStringOperation operator +(OptimizedStringOperation t, bool v)
        {
            t.sb.Append(v);
            return t;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static OptimizedStringOperation operator +(OptimizedStringOperation t, int v)
        {
            t.sb.Append(v);
            return t;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static OptimizedStringOperation operator +(OptimizedStringOperation t, short v)
        {
            t.sb.Append(v);
            return t;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static OptimizedStringOperation operator +(OptimizedStringOperation t, byte v)
        {
            t.sb.Append(v);
            return t;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="v">The v.</param>
        /// <returns>The result of the operator.</returns>
        public static OptimizedStringOperation operator +(OptimizedStringOperation t, float v)
        {
            t.sb.Append(v);
            return t;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="c">The c.</param>
        /// <returns>The result of the operator.</returns>
        public static OptimizedStringOperation operator +(OptimizedStringOperation t, char c)
        {
            t.sb.Append(c);
            return t;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="c">The c.</param>
        /// <returns>The result of the operator.</returns>
        public static OptimizedStringOperation operator +(OptimizedStringOperation t, char[] c)
        {
            t.sb.Append(c);
            return t;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="str">The string.</param>
        /// <returns>The result of the operator.</returns>
        public static OptimizedStringOperation operator +(OptimizedStringOperation t, string str)
        {
            t.sb.Append(str);
            return t;
        }

        /// <summary>
        /// Implements the +.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="sb">The sb.</param>
        /// <returns>The result of the operator.</returns>
        public static OptimizedStringOperation operator +(OptimizedStringOperation t, StringBuilder sb)
        {
            t.sb.Append(sb);
            return t;
        }

        #endregion ADD_OPERATOR
    }
}