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

using UnityInputConverter.YamlDotNet.Core;
using UnityInputConverter.YamlDotNet.Core.Events;

namespace UnityInputConverter.YamlDotNet.Serialization
{
    /// <summary>
    /// Class EventInfo.
    /// </summary>
    public abstract class EventInfo
    {
        /// <summary>
        /// Gets the source.
        /// </summary>
        /// <value>The source.</value>
        public IObjectDescriptor Source { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventInfo"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        protected EventInfo(IObjectDescriptor source)
        {
            Source = source;
        }
    }

    /// <summary>
    /// Class AliasEventInfo.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.EventInfo" />
    public class AliasEventInfo : EventInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AliasEventInfo"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public AliasEventInfo(IObjectDescriptor source)
            : base(source)
        {
        }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        /// <value>The alias.</value>
        public string Alias { get; set; }
    }

    /// <summary>
    /// Class ObjectEventInfo.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.EventInfo" />
    public class ObjectEventInfo : EventInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectEventInfo"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        protected ObjectEventInfo(IObjectDescriptor source)
            : base(source)
        {
        }

        /// <summary>
        /// Gets or sets the anchor.
        /// </summary>
        /// <value>The anchor.</value>
        public string Anchor { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public string Tag { get; set; }
    }

    /// <summary>
    /// Class ScalarEventInfo. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.ObjectEventInfo" />
    public sealed class ScalarEventInfo : ObjectEventInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarEventInfo"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public ScalarEventInfo(IObjectDescriptor source)
            : base(source)
        {
            Style = source.ScalarStyle;
        }

        /// <summary>
        /// Gets or sets the rendered value.
        /// </summary>
        /// <value>The rendered value.</value>
        public string RenderedValue { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public ScalarStyle Style { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is plain implicit.
        /// </summary>
        /// <value><c>true</c> if this instance is plain implicit; otherwise, <c>false</c>.</value>
        public bool IsPlainImplicit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is quoted implicit.
        /// </summary>
        /// <value><c>true</c> if this instance is quoted implicit; otherwise, <c>false</c>.</value>
        public bool IsQuotedImplicit { get; set; }
    }

    /// <summary>
    /// Class MappingStartEventInfo. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.ObjectEventInfo" />
    public sealed class MappingStartEventInfo : ObjectEventInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingStartEventInfo"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public MappingStartEventInfo(IObjectDescriptor source)
            : base(source)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is implicit.
        /// </summary>
        /// <value><c>true</c> if this instance is implicit; otherwise, <c>false</c>.</value>
        public bool IsImplicit { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public MappingStyle Style { get; set; }
    }

    /// <summary>
    /// Class MappingEndEventInfo. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.EventInfo" />
    public sealed class MappingEndEventInfo : EventInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingEndEventInfo"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public MappingEndEventInfo(IObjectDescriptor source)
            : base(source)
        {
        }
    }

    /// <summary>
    /// Class SequenceStartEventInfo. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.ObjectEventInfo" />
    public sealed class SequenceStartEventInfo : ObjectEventInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceStartEventInfo"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public SequenceStartEventInfo(IObjectDescriptor source)
            : base(source)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is implicit.
        /// </summary>
        /// <value><c>true</c> if this instance is implicit; otherwise, <c>false</c>.</value>
        public bool IsImplicit { get; set; }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        /// <value>The style.</value>
        public SequenceStyle Style { get; set; }
    }

    /// <summary>
    /// Class SequenceEndEventInfo. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.EventInfo" />
    public sealed class SequenceEndEventInfo : EventInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceEndEventInfo"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public SequenceEndEventInfo(IObjectDescriptor source)
            : base(source)
        {
        }
    }
}