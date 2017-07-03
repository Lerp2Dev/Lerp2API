//  This file is part of YamlDotNet - A .NET library for YAML.
//  Copyright (c) Antoine Aubry and contributors

//  Permission is hereby granted, free of charge, to any person obtaining a copy of
//  this software and associated documentation files (the "Software"), to deal in
//  the Software without restriction, including without limitation the rights to
//  use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//  of the Software, and to permit persons to whom the Software is furnished to do
//  so, subject to the following conditions:

//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.

//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.

using System;
using UnityInputConverter.YamlDotNet.Core;

namespace UnityInputConverter.YamlDotNet.Serialization
{
    /// <summary>
    /// Class ObjectDescriptor. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.IObjectDescriptor" />
    public sealed class ObjectDescriptor : IObjectDescriptor
    {
        /// <summary>
        /// A reference to the object.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; private set; }

        /// <summary>
        /// The type that should be used when to interpret the <see cref="Value" />.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }

        /// <summary>
        /// The type of <see cref="Value" /> as determined by its container (e.g. a property).
        /// </summary>
        /// <value>The type of the static.</value>
        public Type StaticType { get; private set; }

        /// <summary>
        /// The style to be used for scalars.
        /// </summary>
        /// <value>The scalar style.</value>
        public ScalarStyle ScalarStyle { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDescriptor"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <param name="staticType">Type of the static.</param>
        public ObjectDescriptor(object value, Type type, Type staticType)
            : this(value, type, staticType, ScalarStyle.Any)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDescriptor"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <param name="staticType">Type of the static.</param>
        /// <param name="scalarStyle">The scalar style.</param>
        /// <exception cref="ArgumentNullException">
        /// type
        /// or
        /// staticType
        /// </exception>
        public ObjectDescriptor(object value, Type type, Type staticType, ScalarStyle scalarStyle)
        {
            Value = value;

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            Type = type;

            if (staticType == null)
            {
                throw new ArgumentNullException("staticType");
            }

            StaticType = staticType;

            ScalarStyle = scalarStyle;
        }
    }
}