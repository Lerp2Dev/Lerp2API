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
using System.Collections;
using UnityInputConverter.YamlDotNet.Core;

namespace UnityInputConverter.YamlDotNet.Serialization.NodeDeserializers
{
    /// <summary>
    /// Class ArrayNodeDeserializer. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="UnityInputConverter.YamlDotNet.Serialization.INodeDeserializer" />
    public sealed class ArrayNodeDeserializer : INodeDeserializer
    {
        bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (!expectedType.IsArray)
            {
                value = false;
                return false;
            }

            var itemType = expectedType.GetElementType();

            var items = new ArrayList();
            CollectionNodeDeserializer.DeserializeHelper(itemType, parser, nestedObjectDeserializer, items, true);

            var array = Array.CreateInstance(itemType, items.Count);
            items.CopyTo(array, 0);

            value = array;
            return true;
        }

        private sealed class ArrayList : IList
        {
            private object[] data;
            private int count;

            /// <summary>
            /// Initializes a new instance of the <see cref="ArrayList"/> class.
            /// </summary>
            public ArrayList()
            {
                Clear();
            }

            /// <summary>
            /// Adds the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns>System.Int32.</returns>
            public int Add(object value)
            {
                if (count == data.Length)
                {
                    Array.Resize(ref data, data.Length * 2);
                }
                data[count] = value;
                return count++;
            }

            /// <summary>
            /// Clears this instance.
            /// </summary>
            public void Clear()
            {
                data = new object[10];
                count = 0;
            }

            /// <summary>
            /// Determines whether [contains] [the specified value].
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns><c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.</returns>
            /// <exception cref="NotSupportedException"></exception>
            public bool Contains(object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Indexes the of.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <returns>System.Int32.</returns>
            /// <exception cref="NotSupportedException"></exception>
            public int IndexOf(object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Inserts the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="value">The value.</param>
            /// <exception cref="NotSupportedException"></exception>
            public void Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Gets a value indicating whether this instance is fixed size.
            /// </summary>
            /// <value><c>true</c> if this instance is fixed size; otherwise, <c>false</c>.</value>
            public bool IsFixedSize
            {
                get { return false; }
            }

            /// <summary>
            /// Gets a value indicating whether this instance is read only.
            /// </summary>
            /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
            public bool IsReadOnly
            {
                get { return false; }
            }

            /// <summary>
            /// Removes the specified value.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <exception cref="NotSupportedException"></exception>
            public void Remove(object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Removes at.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <exception cref="NotSupportedException"></exception>
            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Gets or sets the <see cref="System.Object"/> at the specified index.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns>System.Object.</returns>
            public object this[int index]
            {
                get
                {
                    return data[index];
                }
                set
                {
                    data[index] = value;
                }
            }

            /// <summary>
            /// Copies to.
            /// </summary>
            /// <param name="array">The array.</param>
            /// <param name="index">The index.</param>
            public void CopyTo(Array array, int index)
            {
                Array.Copy(data, 0, array, index, count);
            }

            /// <summary>
            /// Gets the count.
            /// </summary>
            /// <value>The count.</value>
            public int Count
            {
                get { return count; }
            }

            /// <summary>
            /// Gets a value indicating whether this instance is synchronized.
            /// </summary>
            /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
            public bool IsSynchronized
            {
                get { return false; }
            }

            /// <summary>
            /// Gets the synchronize root.
            /// </summary>
            /// <value>The synchronize root.</value>
            public object SyncRoot
            {
                get { return data; }
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns>IEnumerator.</returns>
            public IEnumerator GetEnumerator()
            {
                for (int i = 0; i < count; ++i)
                {
                    yield return data[i];
                }
            }
        }
    }
}