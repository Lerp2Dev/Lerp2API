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
using System.IO;
using UnityInputConverter.YamlDotNet.Core;
using UnityInputConverter.YamlDotNet.Core.Events;
using UnityInputConverter.YamlDotNet.Serialization.Converters;
using UnityInputConverter.YamlDotNet.Serialization.NamingConventions;
using UnityInputConverter.YamlDotNet.Serialization.NodeDeserializers;
using UnityInputConverter.YamlDotNet.Serialization.NodeTypeResolvers;
using UnityInputConverter.YamlDotNet.Serialization.ObjectFactories;
using UnityInputConverter.YamlDotNet.Serialization.TypeInspectors;
using UnityInputConverter.YamlDotNet.Serialization.TypeResolvers;
using UnityInputConverter.YamlDotNet.Serialization.Utilities;
using UnityInputConverter.YamlDotNet.Serialization.ValueDeserializers;

namespace UnityInputConverter.YamlDotNet.Serialization
{
    /// <summary>
    /// Deserializes objects from the YAML format.
    /// To customize the behavior of <see cref="Deserializer" />,
    /// use the <see cref="DeserializerBuilder" /> class.
    /// </summary>
    public sealed class Deserializer
    {
        #region Backwards compatibility

        private class BackwardsCompatibleConfiguration
        {
            private static readonly Dictionary<string, Type> predefinedTagMappings = new Dictionary<string, Type>
            {
                { "tag:yaml.org,2002:map", typeof(Dictionary<object, object>) },
                { "tag:yaml.org,2002:bool", typeof(bool) },
                { "tag:yaml.org,2002:float", typeof(double) },
                { "tag:yaml.org,2002:int", typeof(int) },
                { "tag:yaml.org,2002:str", typeof(string) },
                { "tag:yaml.org,2002:timestamp", typeof(DateTime) }
            };

            private readonly Dictionary<string, Type> tagMappings;
            private readonly List<IYamlTypeConverter> converters;
            private TypeDescriptorProxy typeDescriptor = new TypeDescriptorProxy();

            /// <summary>
            /// The value deserializer
            /// </summary>
            public IValueDeserializer valueDeserializer;

            /// <summary>
            /// Gets or sets the node deserializers.
            /// </summary>
            /// <value>The node deserializers.</value>
            public IList<INodeDeserializer> NodeDeserializers { get; private set; }

            /// <summary>
            /// Gets or sets the type resolvers.
            /// </summary>
            /// <value>The type resolvers.</value>
            public IList<INodeTypeResolver> TypeResolvers { get; private set; }

            private class TypeDescriptorProxy : ITypeInspector
            {
                /// <summary>
                /// The type descriptor
                /// </summary>
                public ITypeInspector TypeDescriptor;

                /// <summary>
                /// Gets all properties of the specified type.
                /// </summary>
                /// <param name="type">The type whose properties are to be enumerated.</param>
                /// <param name="container">The actual object of type <paramref name="type" /> whose properties are to be enumerated. Can be null.</param>
                /// <returns>IEnumerable&lt;IPropertyDescriptor&gt;.</returns>
                public IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
                {
                    return TypeDescriptor.GetProperties(type, container);
                }

                /// <summary>
                /// Gets the property of the type with the specified name.
                /// </summary>
                /// <param name="type">The type whose properties are to be searched.</param>
                /// <param name="container">The actual object of type <paramref name="type" /> whose properties are to be searched. Can be null.</param>
                /// <param name="name">The name of the property.</param>
                /// <param name="ignoreUnmatched">Determines if an exception or null should be returned if <paramref name="name" /> can't be
                /// found in <paramref name="type" /></param>
                /// <returns>IPropertyDescriptor.</returns>
                public IPropertyDescriptor GetProperty(Type type, object container, string name, bool ignoreUnmatched)
                {
                    return TypeDescriptor.GetProperty(type, container, name, ignoreUnmatched);
                }
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="BackwardsCompatibleConfiguration"/> class.
            /// </summary>
            /// <param name="objectFactory">The object factory.</param>
            /// <param name="namingConvention">The naming convention.</param>
            /// <param name="ignoreUnmatched">if set to <c>true</c> [ignore unmatched].</param>
            /// <param name="overrides">The overrides.</param>
            public BackwardsCompatibleConfiguration(
                IObjectFactory objectFactory,
                INamingConvention namingConvention,
                bool ignoreUnmatched,
                YamlAttributeOverrides overrides)
            {
                objectFactory = objectFactory ?? new DefaultObjectFactory();
                namingConvention = namingConvention ?? new NullNamingConvention();

                typeDescriptor.TypeDescriptor =
                    new CachedTypeInspector(
                        new NamingConventionTypeInspector(
                            new YamlAttributesTypeInspector(
                                new YamlAttributeOverridesInspector(
                                    new ReadableAndWritablePropertiesTypeInspector(
                                        new ReadablePropertiesTypeInspector(
                                            new StaticTypeResolver()
                                        )
                                    ),
                                    overrides
                                )
                            ),
                            namingConvention
                        )
                    );

                converters = new List<IYamlTypeConverter>();
                converters.Add(new GuidConverter(false));

                NodeDeserializers = new List<INodeDeserializer>();
                NodeDeserializers.Add(new YamlConvertibleNodeDeserializer(objectFactory));
                NodeDeserializers.Add(new YamlSerializableNodeDeserializer(objectFactory));
                NodeDeserializers.Add(new TypeConverterNodeDeserializer(converters));
                NodeDeserializers.Add(new NullNodeDeserializer());
                NodeDeserializers.Add(new ScalarNodeDeserializer());
                NodeDeserializers.Add(new ArrayNodeDeserializer());
                NodeDeserializers.Add(new DictionaryNodeDeserializer(objectFactory));
                NodeDeserializers.Add(new CollectionNodeDeserializer(objectFactory));
                NodeDeserializers.Add(new EnumerableNodeDeserializer());
                NodeDeserializers.Add(new ObjectNodeDeserializer(objectFactory, typeDescriptor, ignoreUnmatched));

                tagMappings = new Dictionary<string, Type>(predefinedTagMappings);
                TypeResolvers = new List<INodeTypeResolver>();
                TypeResolvers.Add(new YamlConvertibleTypeResolver());
                TypeResolvers.Add(new YamlSerializableTypeResolver());
                TypeResolvers.Add(new TagNodeTypeResolver(tagMappings));
                TypeResolvers.Add(new TypeNameInTagNodeTypeResolver());
                TypeResolvers.Add(new DefaultContainersNodeTypeResolver());

                valueDeserializer =
                    new AliasValueDeserializer(
                        new NodeValueDeserializer(
                            NodeDeserializers,
                            TypeResolvers
                        )
                    );
            }

            /// <summary>
            /// Registers the tag mapping.
            /// </summary>
            /// <param name="tag">The tag.</param>
            /// <param name="type">The type.</param>
            public void RegisterTagMapping(string tag, Type type)
            {
                tagMappings.Add(tag, type);
            }

            /// <summary>
            /// Registers the type converter.
            /// </summary>
            /// <param name="typeConverter">The type converter.</param>
            public void RegisterTypeConverter(IYamlTypeConverter typeConverter)
            {
                converters.Insert(0, typeConverter);
            }
        }

        private readonly BackwardsCompatibleConfiguration backwardsCompatibleConfiguration;

        private void ThrowUnlessInBackwardsCompatibleMode()
        {
            if (backwardsCompatibleConfiguration == null)
            {
                throw new InvalidOperationException("This method / property exists for backwards compatibility reasons, but the Deserializer was created using the new configuration mechanism. To configure the Deserializer, use the DeserializerBuilder.");
            }
        }

        /// <summary>
        /// Gets the node deserializers.
        /// </summary>
        /// <value>The node deserializers.</value>
        [Obsolete("Please use DeserializerBuilder to customize the Deserializer. This property will be removed in future releases.")]
        public IList<INodeDeserializer> NodeDeserializers
        {
            get
            {
                ThrowUnlessInBackwardsCompatibleMode();
                return backwardsCompatibleConfiguration.NodeDeserializers;
            }
        }

        /// <summary>
        /// Gets the type resolvers.
        /// </summary>
        /// <value>The type resolvers.</value>
        [Obsolete("Please use DeserializerBuilder to customize the Deserializer. This property will be removed in future releases.")]
        public IList<INodeTypeResolver> TypeResolvers
        {
            get
            {
                ThrowUnlessInBackwardsCompatibleMode();
                return backwardsCompatibleConfiguration.TypeResolvers;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deserializer"/> class.
        /// </summary>
        /// <param name="objectFactory">The object factory.</param>
        /// <param name="namingConvention">The naming convention.</param>
        /// <param name="ignoreUnmatched">if set to <c>true</c> [ignore unmatched].</param>
        /// <param name="overrides">The overrides.</param>
        [Obsolete("Please use DeserializerBuilder to customize the Deserializer. This constructor will be removed in future releases.")]
        public Deserializer(
            IObjectFactory objectFactory = null,
            INamingConvention namingConvention = null,
            bool ignoreUnmatched = false,
            YamlAttributeOverrides overrides = null)
        {
            backwardsCompatibleConfiguration = new BackwardsCompatibleConfiguration(objectFactory, namingConvention, ignoreUnmatched, overrides);
            valueDeserializer = backwardsCompatibleConfiguration.valueDeserializer;
        }

        /// <summary>
        /// Registers the tag mapping.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="type">The type.</param>
        [Obsolete("Please use DeserializerBuilder to customize the Deserializer. This method will be removed in future releases.")]
        public void RegisterTagMapping(string tag, Type type)
        {
            ThrowUnlessInBackwardsCompatibleMode();
            backwardsCompatibleConfiguration.RegisterTagMapping(tag, type);
        }

        /// <summary>
        /// Registers the type converter.
        /// </summary>
        /// <param name="typeConverter">The type converter.</param>
        [Obsolete("Please use DeserializerBuilder to customize the Deserializer. This method will be removed in future releases.")]
        public void RegisterTypeConverter(IYamlTypeConverter typeConverter)
        {
            ThrowUnlessInBackwardsCompatibleMode();
            backwardsCompatibleConfiguration.RegisterTypeConverter(typeConverter);
        }

        #endregion Backwards compatibility

        private readonly IValueDeserializer valueDeserializer;

        /// <summary>
        /// Initializes a new instance of <see cref="Deserializer" /> using the default configuration.
        /// </summary>
        /// <remarks>
        /// To customize the bahavior of the deserializer, use <see cref="DeserializerBuilder" />.
        /// </remarks>
        public Deserializer()
        // TODO: When the backwards compatibility is dropped, uncomment the following line and remove the body of this constructor.
        // : this(new DeserializerBuilder().BuildValueDeserializer())
        {
            backwardsCompatibleConfiguration = new BackwardsCompatibleConfiguration(null, null, false, null);
            valueDeserializer = backwardsCompatibleConfiguration.valueDeserializer;
        }

        /// <remarks>
        /// This constructor is private to discourage its use.
        /// To invoke it, call the <see cref="FromValueDeserializer"/> method.
        /// </remarks>
        private Deserializer(IValueDeserializer valueDeserializer)
        {
            if (valueDeserializer == null)
            {
                throw new ArgumentNullException("valueDeserializer");
            }

            this.valueDeserializer = valueDeserializer;
        }

        /// <summary>
        /// Creates a new <see cref="Deserializer" /> that uses the specified <see cref="IValueDeserializer" />.
        /// This method is available for advanced scenarios. The preferred way to customize the bahavior of the
        /// deserializer is to use <see cref="DeserializerBuilder" />.
        /// </summary>
        public static Deserializer FromValueDeserializer(IValueDeserializer valueDeserializer)
        {
            return new Deserializer(valueDeserializer);
        }

        /// <summary>
        /// Deserializes the specified input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <returns>T.</returns>
        public T Deserialize<T>(string input)
        {
            using (var reader = new StringReader(input))
            {
                return (T)Deserialize(reader, typeof(T));
            }
        }

        /// <summary>
        /// Deserializes the specified input.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <returns>T.</returns>
        public T Deserialize<T>(TextReader input)
        {
            return (T)Deserialize(input, typeof(T));
        }

        /// <summary>
        /// Deserializes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.Object.</returns>
        public object Deserialize(TextReader input)
        {
            return Deserialize(input, typeof(object));
        }

        /// <summary>
        /// Deserializes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object Deserialize(string input, Type type)
        {
            using (var reader = new StringReader(input))
            {
                return Deserialize(reader, type);
            }
        }

        /// <summary>
        /// Deserializes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object Deserialize(TextReader input, Type type)
        {
            return Deserialize(new Parser(input), type);
        }

        /// <summary>
        /// Deserializes the specified parser.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>T.</returns>
        public T Deserialize<T>(IParser parser)
        {
            return (T)Deserialize(parser, typeof(T));
        }

        /// <summary>
        /// Deserializes the specified parser.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <returns>System.Object.</returns>
        public object Deserialize(IParser parser)
        {
            return Deserialize(parser, typeof(object));
        }

        /// <summary>
        /// Deserializes an object of the specified type.
        /// </summary>
        /// <param name="parser">The <see cref="IParser" /> from where to deserialize the object.</param>
        /// <param name="type">The static type of the object to deserialize.</param>
        /// <returns>Returns the deserialized object.</returns>
        public object Deserialize(IParser parser, Type type)
        {
            if (parser == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var hasStreamStart = parser.Allow<StreamStart>() != null;

            var hasDocumentStart = parser.Allow<DocumentStart>() != null;

            object result = null;
            if (!parser.Accept<DocumentEnd>() && !parser.Accept<StreamEnd>())
            {
                using (var state = new SerializerState())
                {
                    result = valueDeserializer.DeserializeValue(parser, type, state, valueDeserializer);
                    state.OnDeserialization();
                }
            }

            if (hasDocumentStart)
            {
                parser.Expect<DocumentEnd>();
            }

            if (hasStreamStart)
            {
                parser.Expect<StreamEnd>();
            }

            return result;
        }
    }
}