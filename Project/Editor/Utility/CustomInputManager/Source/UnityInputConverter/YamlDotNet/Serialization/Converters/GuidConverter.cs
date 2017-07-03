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

namespace UnityInputConverter.YamlDotNet.Serialization.Converters
{
    /// <summary>
    /// Converter for System.Guid.
    /// </summary>
    public class GuidConverter : IYamlTypeConverter
    {
        private readonly bool jsonCompatible;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidConverter"/> class.
        /// </summary>
        /// <param name="jsonCompatible">if set to <c>true</c> [json compatible].</param>
        public GuidConverter(bool jsonCompatible)
        {
            this.jsonCompatible = jsonCompatible;
        }

        /// <summary>
        /// Gets a value indicating whether the current converter supports converting the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Accepts(System.Type type)
        {
            return type == typeof(System.Guid);
        }

        /// <summary>
        /// Reads an object's state from a YAML parser.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object ReadYaml(Core.IParser parser, System.Type type)
        {
            var value = ((Core.Events.Scalar)parser.Current).Value;
            parser.MoveNext();
            return new System.Guid(value);
        }

        /// <summary>
        /// Writes the specified object's state to a YAML emitter.
        /// </summary>
        /// <param name="emitter">The emitter.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        public void WriteYaml(Core.IEmitter emitter, object value, System.Type type)
        {
            var guid = (System.Guid)value;
            emitter.Emit(new Core.Events.Scalar(null, null, guid.ToString("D"), jsonCompatible ? ScalarStyle.DoubleQuoted : ScalarStyle.Any, true, false));
        }
    }
}