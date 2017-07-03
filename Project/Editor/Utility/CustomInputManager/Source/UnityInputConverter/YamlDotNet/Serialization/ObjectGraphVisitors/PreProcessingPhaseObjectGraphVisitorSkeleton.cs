﻿//  This file is part of YamlDotNet - A .NET library for YAML.
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

namespace UnityInputConverter.YamlDotNet.Serialization.ObjectGraphVisitors
{
    /// <summary>
    /// A base class that simplifies the correct implementation of <see cref="IObjectGraphVisitor{Nothing}" />.
    /// </summary>
    public abstract class PreProcessingPhaseObjectGraphVisitorSkeleton : IObjectGraphVisitor<Nothing>
    {
        /// <summary>
        /// The type converters
        /// </summary>
        protected readonly IEnumerable<IYamlTypeConverter> typeConverters;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreProcessingPhaseObjectGraphVisitorSkeleton"/> class.
        /// </summary>
        /// <param name="typeConverters">The type converters.</param>
        public PreProcessingPhaseObjectGraphVisitorSkeleton(IEnumerable<IYamlTypeConverter> typeConverters)
        {
            this.typeConverters = typeConverters != null
                ? typeConverters.ToList()
                : Enumerable.Empty<IYamlTypeConverter>();
        }

        bool IObjectGraphVisitor<Nothing>.Enter(IObjectDescriptor value, Nothing context)
        {
            var typeConverter = typeConverters.FirstOrDefault(t => t.Accepts(value.Type));
            if (typeConverter != null)
            {
                return false;
            }

            var convertible = value.Value as IYamlConvertible;
            if (convertible != null)
            {
                return false;
            }

#pragma warning disable 0618 // IYamlSerializable is obsolete
            var serializable = value.Value as IYamlSerializable;
            if (serializable != null)
            {
                return false;
            }
#pragma warning restore

            return Enter(value);
        }

        bool IObjectGraphVisitor<Nothing>.EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, Nothing context)
        {
            return EnterMapping(key, value);
        }

        bool IObjectGraphVisitor<Nothing>.EnterMapping(IObjectDescriptor key, IObjectDescriptor value, Nothing context)
        {
            return EnterMapping(key, value);
        }

        void IObjectGraphVisitor<Nothing>.VisitMappingEnd(IObjectDescriptor mapping, Nothing context)
        {
            VisitMappingEnd(mapping);
        }

        void IObjectGraphVisitor<Nothing>.VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType, Nothing context)
        {
            VisitMappingStart(mapping, keyType, valueType);
        }

        void IObjectGraphVisitor<Nothing>.VisitScalar(IObjectDescriptor scalar, Nothing context)
        {
            VisitScalar(scalar);
        }

        void IObjectGraphVisitor<Nothing>.VisitSequenceEnd(IObjectDescriptor sequence, Nothing context)
        {
            VisitSequenceEnd(sequence);
        }

        void IObjectGraphVisitor<Nothing>.VisitSequenceStart(IObjectDescriptor sequence, Type elementType, Nothing context)
        {
            VisitSequenceStart(sequence, elementType);
        }

        /// <summary>
        /// Enters the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected abstract bool Enter(IObjectDescriptor value);

        /// <summary>
        /// Enters the mapping.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected abstract bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value);

        /// <summary>
        /// Enters the mapping.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected abstract bool EnterMapping(IObjectDescriptor key, IObjectDescriptor value);

        /// <summary>
        /// Visits the mapping end.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        protected abstract void VisitMappingEnd(IObjectDescriptor mapping);

        /// <summary>
        /// Visits the mapping start.
        /// </summary>
        /// <param name="mapping">The mapping.</param>
        /// <param name="keyType">Type of the key.</param>
        /// <param name="valueType">Type of the value.</param>
        protected abstract void VisitMappingStart(IObjectDescriptor mapping, Type keyType, Type valueType);

        /// <summary>
        /// Visits the scalar.
        /// </summary>
        /// <param name="scalar">The scalar.</param>
        protected abstract void VisitScalar(IObjectDescriptor scalar);

        /// <summary>
        /// Visits the sequence end.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        protected abstract void VisitSequenceEnd(IObjectDescriptor sequence);

        /// <summary>
        /// Visits the sequence start.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="elementType">Type of the element.</param>
        protected abstract void VisitSequenceStart(IObjectDescriptor sequence, Type elementType);
    }
}