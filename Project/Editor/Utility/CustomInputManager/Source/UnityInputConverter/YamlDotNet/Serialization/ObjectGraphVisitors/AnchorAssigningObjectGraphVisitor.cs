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
using UnityInputConverter.YamlDotNet.Core;

namespace UnityInputConverter.YamlDotNet.Serialization.ObjectGraphVisitors
{
    /// <summary>
    /// Class AnchorAssigningObjectGraphVisitor. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.ObjectGraphVisitors.ChainedObjectGraphVisitor" />
    public sealed class AnchorAssigningObjectGraphVisitor : ChainedObjectGraphVisitor
    {
        private readonly IEventEmitter eventEmitter;
        private readonly IAliasProvider aliasProvider;
        private readonly HashSet<string> emittedAliases = new HashSet<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AnchorAssigningObjectGraphVisitor"/> class.
        /// </summary>
        /// <param name="nextVisitor">The next visitor.</param>
        /// <param name="eventEmitter">The event emitter.</param>
        /// <param name="aliasProvider">The alias provider.</param>
        public AnchorAssigningObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor, IEventEmitter eventEmitter, IAliasProvider aliasProvider)
            : base(nextVisitor)
        {
            this.eventEmitter = eventEmitter;
            this.aliasProvider = aliasProvider;
        }

        /// <summary>
        /// Indicates whether the specified value should be entered. This allows the visitor to
        /// override the handling of a particular object or type.
        /// </summary>
        /// <param name="value">The value that is about to be entered.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        /// <returns>If the value is to be entered, returns true; otherwise returns false;</returns>
        public override bool Enter(IObjectDescriptor value, IEmitter context)
        {
            var alias = aliasProvider.GetAlias(value.Value);
            if (alias != null && !emittedAliases.Add(alias))
            {
                eventEmitter.Emit(new AliasEventInfo(value) { Alias = alias }, context);
                return false;
            }

            return base.Enter(value, context);
        }

        /// <summary>
        /// Notifies the visitor that the traversal of a mapping is about to begin.
        /// </summary>
        /// <param name="mapping">The value that corresponds to the mapping.</param>
        /// <param name="keyType">The static type of the keys of the mapping.</param>
        /// <param name="valueType">The static type of the values of the mapping.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        public override void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType, IEmitter context)
        {
            eventEmitter.Emit(new MappingStartEventInfo(mapping) { Anchor = aliasProvider.GetAlias(mapping.Value) }, context);
        }

        /// <summary>
        /// Notifies the visitor that the traversal of a sequence is about to begin.
        /// </summary>
        /// <param name="sequence">The value that corresponds to the sequence.</param>
        /// <param name="elementType">The static type of the elements of the sequence.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        public override void VisitSequenceStart(IObjectDescriptor sequence, Type elementType, IEmitter context)
        {
            eventEmitter.Emit(new SequenceStartEventInfo(sequence) { Anchor = aliasProvider.GetAlias(sequence.Value) }, context);
        }

        /// <summary>
        /// Notifies the visitor that a scalar value has been encountered.
        /// </summary>
        /// <param name="scalar">The value of the scalar.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        public override void VisitScalar(IObjectDescriptor scalar, IEmitter context)
        {
            eventEmitter.Emit(new ScalarEventInfo(scalar) { Anchor = aliasProvider.GetAlias(scalar.Value) }, context);
        }
    }
}