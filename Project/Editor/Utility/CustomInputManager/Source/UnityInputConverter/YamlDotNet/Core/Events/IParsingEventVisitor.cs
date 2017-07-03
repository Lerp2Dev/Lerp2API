//  This file is part of YamlDotNet - A .NET library for YAML.
//  Copyright (c) 2014 Leon Mlakar
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
    /// Callback interface for external event Visitor.
    /// </summary>
    public interface IParsingEventVisitor
    {
        /// <summary>
        /// Visits the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        void Visit(AnchorAlias e);

        /// <summary>
        /// Visits the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        void Visit(StreamStart e);

        /// <summary>
        /// Visits the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        void Visit(StreamEnd e);

        /// <summary>
        /// Visits the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        void Visit(DocumentStart e);

        /// <summary>
        /// Visits the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        void Visit(DocumentEnd e);

        /// <summary>
        /// Visits the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        void Visit(Scalar e);

        /// <summary>
        /// Visits the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        void Visit(SequenceStart e);

        /// <summary>
        /// Visits the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        void Visit(SequenceEnd e);

        /// <summary>
        /// Visits the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        void Visit(MappingStart e);

        /// <summary>
        /// Visits the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        void Visit(MappingEnd e);

        /// <summary>
        /// Visits the specified e.
        /// </summary>
        /// <param name="e">The e.</param>
        void Visit(Comment e);
    }
}