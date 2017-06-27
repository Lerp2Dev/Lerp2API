using System;

namespace FullSerializer.Internal
{
    /// <summary>
    /// Simple option type. This is akin to nullable types.
    /// </summary>
    public struct fsOption<T>
    {
        private bool _hasValue;
        private T _value;

        /// <summary>
        /// Gets a value indicating whether this instance has value.
        /// </summary>
        /// <value><c>true</c> if this instance has value; otherwise, <c>false</c>.</value>
        public bool HasValue
        {
            get { return _hasValue; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return _hasValue == false; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        /// <exception cref="InvalidOperationException">fsOption is empty</exception>
        public T Value
        {
            get
            {
                if (IsEmpty) throw new InvalidOperationException("fsOption is empty");
                return _value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="fsOption{T}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public fsOption(T value)
        {
            _hasValue = true;
            _value = value;
        }

        /// <summary>
        /// The empty
        /// </summary>
        public static fsOption<T> Empty;
    }

    /// <summary>
    /// Class fsOption.
    /// </summary>
    public static class fsOption
    {
        /// <summary>
        /// Justs the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns>fsOption&lt;T&gt;.</returns>
        public static fsOption<T> Just<T>(T value)
        {
            return new fsOption<T>(value);
        }
    }
}