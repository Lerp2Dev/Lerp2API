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

namespace UnityInputConverter.YamlDotNet.Core.Events
{
    /// <summary>
    /// Class Comment.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Core.Events.ParsingEvent" />
    public class Comment : ParsingEvent
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is inline.
        /// </summary>
        /// <value><c>true</c> if this instance is inline; otherwise, <c>false</c>.</value>
        public bool IsInline { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comment"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="isInline">if set to <c>true</c> [is inline].</param>
        public Comment(string value, bool isInline)
            : this(value, isInline, Mark.Empty, Mark.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comment"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="isInline">if set to <c>true</c> [is inline].</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        public Comment(string value, bool isInline, Mark start, Mark end)
            : base(start, end)
        {
            Value = value;
            IsInline = isInline;
        }

        internal override EventType Type
        {
            get { return EventType.Comment; }
        }

        /// <summary>
        /// Accepts the specified visitor.
        /// </summary>
        /// <param name="visitor">Visitor to accept, may not be null</param>
        public override void Accept(IParsingEventVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}