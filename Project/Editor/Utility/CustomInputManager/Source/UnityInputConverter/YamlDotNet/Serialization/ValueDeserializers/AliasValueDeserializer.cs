// This file is part of YamlDotNet - A .NET library for YAML.
// Copyright (c) Antoine Aubry and contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using UnityInputConverter.YamlDotNet.Core;
using UnityInputConverter.YamlDotNet.Core.Events;
using UnityInputConverter.YamlDotNet.Serialization.Utilities;

namespace UnityInputConverter.YamlDotNet.Serialization.ValueDeserializers
{
    /// <summary>
    /// Class AliasValueDeserializer. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.IValueDeserializer" />
    public sealed class AliasValueDeserializer : IValueDeserializer
    {
        private readonly IValueDeserializer innerDeserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="AliasValueDeserializer"/> class.
        /// </summary>
        /// <param name="innerDeserializer">The inner deserializer.</param>
        /// <exception cref="ArgumentNullException">innerDeserializer</exception>
        public AliasValueDeserializer(IValueDeserializer innerDeserializer)
        {
            if (innerDeserializer == null)
            {
                throw new ArgumentNullException("innerDeserializer");
            }

            this.innerDeserializer = innerDeserializer;
        }

        private sealed class AliasState : Dictionary<string, ValuePromise>, IPostDeserializationCallback
        {
            /// <summary>
            /// Called when [deserialization].
            /// </summary>
            /// <exception cref="AnchorNotFoundException"></exception>
            public void OnDeserialization()
            {
                foreach (var promise in Values)
                {
                    if (!promise.HasValue)
                    {
                        throw new AnchorNotFoundException(promise.Alias.Start, promise.Alias.End, string.Format(
                            "Anchor '{0}' not found",
                            promise.Alias.Value
                        ));
                    }
                }
            }
        }

        private sealed class ValuePromise : IValuePromise
        {
            /// <summary>
            /// Occurs when [value available].
            /// </summary>
            public event Action<object> ValueAvailable;

            /// <summary>
            /// Gets or sets a value indicating whether this instance has value.
            /// </summary>
            /// <value><c>true</c> if this instance has value; otherwise, <c>false</c>.</value>
            public bool HasValue { get; private set; }

            private object value;

            /// <summary>
            /// The alias
            /// </summary>
            public readonly AnchorAlias Alias;

            /// <summary>
            /// Initializes a new instance of the <see cref="ValuePromise"/> class.
            /// </summary>
            /// <param name="alias">The alias.</param>
            public ValuePromise(AnchorAlias alias)
            {
                this.Alias = alias;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ValuePromise"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public ValuePromise(object value)
            {
                HasValue = true;
                this.value = value;
            }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            /// <exception cref="InvalidOperationException">
            /// Value not set
            /// or
            /// Value already set
            /// </exception>
            public object Value
            {
                get
                {
                    if (!HasValue)
                    {
                        throw new InvalidOperationException("Value not set");
                    }
                    return value;
                }
                set
                {
                    if (HasValue)
                    {
                        throw new InvalidOperationException("Value already set");
                    }
                    HasValue = true;
                    this.value = value;

                    if (ValueAvailable != null)
                    {
                        ValueAvailable(value);
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes the value.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="state">The state.</param>
        /// <param name="nestedObjectDeserializer">The nested object deserializer.</param>
        /// <returns>System.Object.</returns>
        public object DeserializeValue(IParser parser, Type expectedType, SerializerState state, IValueDeserializer nestedObjectDeserializer)
        {
            object value;
            var alias = parser.Allow<AnchorAlias>();
            if (alias != null)
            {
                var aliasState = state.Get<AliasState>();
                ValuePromise valuePromise;
                if (!aliasState.TryGetValue(alias.Value, out valuePromise))
                {
                    valuePromise = new ValuePromise(alias);
                    aliasState.Add(alias.Value, valuePromise);
                }

                return valuePromise.HasValue ? valuePromise.Value : valuePromise;
            }

            string anchor = null;

            var nodeEvent = parser.Peek<NodeEvent>();
            if (nodeEvent != null && !string.IsNullOrEmpty(nodeEvent.Anchor))
            {
                anchor = nodeEvent.Anchor;
            }

            value = innerDeserializer.DeserializeValue(parser, expectedType, state, nestedObjectDeserializer);

            if (anchor != null)
            {
                var aliasState = state.Get<AliasState>();

                ValuePromise valuePromise;
                if (!aliasState.TryGetValue(anchor, out valuePromise))
                {
                    aliasState.Add(anchor, new ValuePromise(value));
                }
                else if (!valuePromise.HasValue)
                {
                    valuePromise.Value = value;
                }
                else
                {
                    aliasState[anchor] = new ValuePromise(value);
                }
            }

            return value;
        }
    }
}