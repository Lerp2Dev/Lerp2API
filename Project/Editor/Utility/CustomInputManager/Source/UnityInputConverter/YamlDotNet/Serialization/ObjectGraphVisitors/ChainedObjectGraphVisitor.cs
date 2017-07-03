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

namespace UnityInputConverter.YamlDotNet.Serialization.ObjectGraphVisitors
{
    /// <summary>
    /// Class ChainedObjectGraphVisitor.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.IObjectGraphVisitor{UnityInputConverter.YamlDotNet.Core.IEmitter}" />
    public abstract class ChainedObjectGraphVisitor : IObjectGraphVisitor<IEmitter>
    {
        private readonly IObjectGraphVisitor<IEmitter> nextVisitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChainedObjectGraphVisitor"/> class.
        /// </summary>
        /// <param name="nextVisitor">The next visitor.</param>
        protected ChainedObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
        {
            this.nextVisitor = nextVisitor;
        }

        /// <summary>
        /// Indicates whether the specified value should be entered. This allows the visitor to
        /// override the handling of a particular object or type.
        /// </summary>
        /// <param name="value">The value that is about to be entered.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        /// <returns>If the value is to be entered, returns true; otherwise returns false;</returns>
        public virtual bool Enter(IObjectDescriptor value, IEmitter context)
        {
            return nextVisitor.Enter(value, context);
        }

        /// <summary>
        /// Indicates whether the specified mapping should be entered. This allows the visitor to
        /// override the handling of a particular pair.
        /// </summary>
        /// <param name="key">The key of the mapping that is about to be entered.</param>
        /// <param name="value">The value of the mapping that is about to be entered.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        /// <returns>If the mapping is to be entered, returns true; otherwise returns false;</returns>
        public virtual bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            return nextVisitor.EnterMapping(key, value, context);
        }

        /// <summary>
        /// Indicates whether the specified mapping should be entered. This allows the visitor to
        /// override the handling of a particular pair. This overload should be invoked when the
        /// mapping is produced by an object's property.
        /// </summary>
        /// <param name="key">The <see cref="T:UnityInputConverter.YamlDotNet.Serialization.IPropertyDescriptor" /> that provided access to <paramref name="value" />.</param>
        /// <param name="value">The value of the mapping that is about to be entered.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        /// <returns>If the mapping is to be entered, returns true; otherwise returns false;</returns>
        public virtual bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            return nextVisitor.EnterMapping(key, value, context);
        }

        /// <summary>
        /// Notifies the visitor that a scalar value has been encountered.
        /// </summary>
        /// <param name="scalar">The value of the scalar.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        public virtual void VisitScalar(IObjectDescriptor scalar, IEmitter context)
        {
            nextVisitor.VisitScalar(scalar, context);
        }

        /// <summary>
        /// Notifies the visitor that the traversal of a mapping is about to begin.
        /// </summary>
        /// <param name="mapping">The value that corresponds to the mapping.</param>
        /// <param name="keyType">The static type of the keys of the mapping.</param>
        /// <param name="valueType">The static type of the values of the mapping.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        public virtual void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType, IEmitter context)
        {
            nextVisitor.VisitMappingStart(mapping, keyType, valueType, context);
        }

        /// <summary>
        /// Notifies the visitor that the traversal of a mapping has ended.
        /// </summary>
        /// <param name="mapping">The value that corresponds to the mapping.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        public virtual void VisitMappingEnd(IObjectDescriptor mapping, IEmitter context)
        {
            nextVisitor.VisitMappingEnd(mapping, context);
        }

        /// <summary>
        /// Notifies the visitor that the traversal of a sequence is about to begin.
        /// </summary>
        /// <param name="sequence">The value that corresponds to the sequence.</param>
        /// <param name="elementType">The static type of the elements of the sequence.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        public virtual void VisitSequenceStart(IObjectDescriptor sequence, Type elementType, IEmitter context)
        {
            nextVisitor.VisitSequenceStart(sequence, elementType, context);
        }

        /// <summary>
        /// Notifies the visitor that the traversal of a sequence has ended.
        /// </summary>
        /// <param name="sequence">The value that corresponds to the sequence.</param>
        /// <param name="context">The context that this implementation depend on.</param>
        public virtual void VisitSequenceEnd(IObjectDescriptor sequence, IEmitter context)
        {
            nextVisitor.VisitSequenceEnd(sequence, context);
        }
    }
}