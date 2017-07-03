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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityInputConverter.YamlDotNet.Core;

namespace UnityInputConverter.YamlDotNet.Serialization.TypeInspectors
{
    /// <summary>
    /// Returns the properties of a type that are readable.
    /// </summary>
    public sealed class ReadablePropertiesTypeInspector : TypeInspectorSkeleton
    {
        private readonly ITypeResolver _typeResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadablePropertiesTypeInspector"/> class.
        /// </summary>
        /// <param name="typeResolver">The type resolver.</param>
        /// <exception cref="ArgumentNullException">typeResolver</exception>
        public ReadablePropertiesTypeInspector(ITypeResolver typeResolver)
        {
            if (typeResolver == null)
            {
                throw new ArgumentNullException("typeResolver");
            }

            _typeResolver = typeResolver;
        }

        private static bool IsValidProperty(PropertyInfo property)
        {
            return property.CanRead
                && property.GetGetMethod().GetParameters().Length == 0;
        }

        /// <summary>
        /// Gets all properties of the specified type.
        /// </summary>
        /// <param name="type">The type whose properties are to be enumerated.</param>
        /// <param name="container">The actual object of type <paramref name="type" /> whose properties are to be enumerated. Can be null.</param>
        /// <returns>IEnumerable&lt;IPropertyDescriptor&gt;.</returns>
        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
        {
            return type
                .GetPublicProperties()
                .Where(IsValidProperty)
                .Select(p => (IPropertyDescriptor)new ReflectionPropertyDescriptor(p, _typeResolver));
        }

        private sealed class ReflectionPropertyDescriptor : IPropertyDescriptor
        {
            private readonly PropertyInfo _propertyInfo;
            private readonly ITypeResolver _typeResolver;

            /// <summary>
            /// Initializes a new instance of the <see cref="ReflectionPropertyDescriptor"/> class.
            /// </summary>
            /// <param name="propertyInfo">The property information.</param>
            /// <param name="typeResolver">The type resolver.</param>
            public ReflectionPropertyDescriptor(PropertyInfo propertyInfo, ITypeResolver typeResolver)
            {
                _propertyInfo = propertyInfo;
                _typeResolver = typeResolver;
                ScalarStyle = ScalarStyle.Any;
            }

            /// <summary>
            /// Gets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get { return _propertyInfo.Name; } }

            /// <summary>
            /// Gets the type.
            /// </summary>
            /// <value>The type.</value>
            public Type Type { get { return _propertyInfo.PropertyType; } }

            /// <summary>
            /// Gets or sets the type override.
            /// </summary>
            /// <value>The type override.</value>
            public Type TypeOverride { get; set; }

            /// <summary>
            /// Gets or sets the order.
            /// </summary>
            /// <value>The order.</value>
            public int Order { get; set; }

            /// <summary>
            /// Gets a value indicating whether this instance can write.
            /// </summary>
            /// <value><c>true</c> if this instance can write; otherwise, <c>false</c>.</value>
            public bool CanWrite { get { return _propertyInfo.CanWrite; } }

            /// <summary>
            /// Gets or sets the scalar style.
            /// </summary>
            /// <value>The scalar style.</value>
            public ScalarStyle ScalarStyle { get; set; }

            /// <summary>
            /// Writes the specified target.
            /// </summary>
            /// <param name="target">The target.</param>
            /// <param name="value">The value.</param>
            public void Write(object target, object value)
            {
                _propertyInfo.SetValue(target, value, null);
            }

            /// <summary>
            /// Gets the custom attribute.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns>T.</returns>
            public T GetCustomAttribute<T>() where T : Attribute
            {
                var attributes = _propertyInfo.GetCustomAttributes(typeof(T), true);
                return (T)attributes.FirstOrDefault();
            }

            /// <summary>
            /// Reads the specified target.
            /// </summary>
            /// <param name="target">The target.</param>
            /// <returns>IObjectDescriptor.</returns>
            public IObjectDescriptor Read(object target)
            {
                var propertyValue = _propertyInfo.ReadValue(target);
                var actualType = TypeOverride ?? _typeResolver.Resolve(Type, propertyValue);
                return new ObjectDescriptor(propertyValue, actualType, Type, ScalarStyle);
            }
        }
    }
}