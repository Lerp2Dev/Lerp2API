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
using UnityInputConverter.YamlDotNet.Core;
using UnityInputConverter.YamlDotNet.Serialization.TypeInspectors;

namespace UnityInputConverter.YamlDotNet.Serialization
{
    /// <summary>
    /// Applies the Yaml attribute overrides to another <see cref="ITypeInspector"/>.
    /// </summary>
    public sealed class YamlAttributeOverridesInspector : TypeInspectorSkeleton
    {
        private readonly ITypeInspector innerTypeDescriptor;
        private readonly YamlAttributeOverrides overrides;

        /// <summary>
        /// Initializes a new instance of the <see cref="YamlAttributeOverridesInspector"/> class.
        /// </summary>
        /// <param name="innerTypeDescriptor">The inner type descriptor.</param>
        /// <param name="overrides">The overrides.</param>
        public YamlAttributeOverridesInspector(ITypeInspector innerTypeDescriptor, YamlAttributeOverrides overrides)
        {
            this.innerTypeDescriptor = innerTypeDescriptor;
            this.overrides = overrides;
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="container">The container.</param>
        /// <returns>IEnumerable&lt;IPropertyDescriptor&gt;.</returns>
        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
        {
            if (overrides == null)
                return innerTypeDescriptor.GetProperties(type, container);

            return innerTypeDescriptor.GetProperties(type, container)
                .Select(p => (IPropertyDescriptor)new OverridePropertyDescriptor(p, overrides, type));
        }

        /// <summary>
        /// Class OverridePropertyDescriptor. This class cannot be inherited.
        /// </summary>
        /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.IPropertyDescriptor" />
        public sealed class OverridePropertyDescriptor : IPropertyDescriptor
        {
            private readonly IPropertyDescriptor baseDescriptor;
            private readonly YamlAttributeOverrides overrides;
            private readonly Type classType;

            /// <summary>
            /// Initializes a new instance of the <see cref="OverridePropertyDescriptor"/> class.
            /// </summary>
            /// <param name="baseDescriptor">The base descriptor.</param>
            /// <param name="overrides">The overrides.</param>
            /// <param name="classType">Type of the class.</param>
            public OverridePropertyDescriptor(IPropertyDescriptor baseDescriptor, YamlAttributeOverrides overrides, Type classType)
            {
                this.baseDescriptor = baseDescriptor;
                this.overrides = overrides;
                this.classType = classType;
            }

            /// <summary>
            /// Gets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get { return baseDescriptor.Name; } }

            /// <summary>
            /// Gets a value indicating whether this instance can write.
            /// </summary>
            /// <value><c>true</c> if this instance can write; otherwise, <c>false</c>.</value>
            public bool CanWrite { get { return baseDescriptor.CanWrite; } }

            /// <summary>
            /// Gets the type.
            /// </summary>
            /// <value>The type.</value>
            public Type Type { get { return baseDescriptor.Type; } }

            /// <summary>
            /// Gets or sets the type override.
            /// </summary>
            /// <value>The type override.</value>
            public Type TypeOverride
            {
                get { return baseDescriptor.TypeOverride; }
                set { baseDescriptor.TypeOverride = value; }
            }

            /// <summary>
            /// Gets or sets the order.
            /// </summary>
            /// <value>The order.</value>
            public int Order
            {
                get { return baseDescriptor.Order; }
                set { baseDescriptor.Order = value; }
            }

            /// <summary>
            /// Gets or sets the scalar style.
            /// </summary>
            /// <value>The scalar style.</value>
            public ScalarStyle ScalarStyle
            {
                get { return baseDescriptor.ScalarStyle; }
                set { baseDescriptor.ScalarStyle = value; }
            }

            /// <summary>
            /// Writes the specified target.
            /// </summary>
            /// <param name="target">The target.</param>
            /// <param name="value">The value.</param>
            public void Write(object target, object value)
            {
                baseDescriptor.Write(target, value);
            }

            /// <summary>
            /// Gets the custom attribute.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns>T.</returns>
            public T GetCustomAttribute<T>() where T : Attribute
            {
                var attr = overrides.GetAttribute<T>(classType, Name);
                return attr ?? baseDescriptor.GetCustomAttribute<T>();
            }

            /// <summary>
            /// Reads the specified target.
            /// </summary>
            /// <param name="target">The target.</param>
            /// <returns>IObjectDescriptor.</returns>
            public IObjectDescriptor Read(object target)
            {
                return baseDescriptor.Read(target);
            }
        }
    }
}