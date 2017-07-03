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
using System.Globalization;

namespace UnityInputConverter.YamlDotNet.Serialization.ObjectGraphVisitors
{
    /// <summary>
    /// Class AnchorAssigner. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.ObjectGraphVisitors.PreProcessingPhaseObjectGraphVisitorSkeleton" />
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.IAliasProvider" />
    public sealed class AnchorAssigner : PreProcessingPhaseObjectGraphVisitorSkeleton, IAliasProvider
    {
        private class AnchorAssignment
        {
            /// <summary>
            /// The anchor
            /// </summary>
            public string Anchor;
        }

        private readonly IDictionary<object, AnchorAssignment> assignments = new Dictionary<object, AnchorAssignment>();
        private uint nextId;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchorAssigner"/> class.
        /// </summary>
        /// <param name="typeConverters">The type converters.</param>
        public AnchorAssigner(IEnumerable<IYamlTypeConverter> typeConverters)
            : base(typeConverters)
        {
        }

        /// <summary>
        /// Enters the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected override bool Enter(IObjectDescriptor value)
        {
            AnchorAssignment assignment;
            if (value.Value != null && assignments.TryGetValue(value.Value, out assignment))
            {
                if (assignment.Anchor == null)
                {
                    assignment.Anchor = "o" + nextId.ToString(CultureInfo.InvariantCulture);
                    ++nextId;
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Enters the mapping.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected override bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value)
        {
            return true;
        }

        /// <summary>
        /// Enters the mapping.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value)
        {
            return true;
        }

        /// <summary>
        /// Visits the scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        protected override void VisitScalar(IObjectDescriptor scalar)
        {
            // Do not assign anchors to scalars
        }

        /// <summary>
        /// Visits the mapping start.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="valueType">Type of the value.</param>
        protected override void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType)
        {
            VisitObject(mapping);
        }

        /// <summary>
        /// Visits the mapping end.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        protected override void VisitMappingEnd(IObjectDescriptor mapping) { }

        /// <summary>
        /// Visits the sequence start.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="elementType">Type of the element.</param>
        protected override void VisitSequenceStart(IObjectDescriptor sequence, Type elementType)
        {
            VisitObject(sequence);
        }

        /// <summary>
        /// Visits the sequence end.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        protected override void VisitSequenceEnd(IObjectDescriptor sequence) { }

        private void VisitObject(IObjectDescriptor value)
        {
            if (value.Value != null)
            {
                assignments.Add(value.Value, new AnchorAssignment());
            }
        }

        string IAliasProvider.GetAlias(object target)
        {
            AnchorAssignment assignment;
            if (target != null && assignments.TryGetValue(target, out assignment))
            {
                return assignment.Anchor;
            }
            return null;
        }
    }
}