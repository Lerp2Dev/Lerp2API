using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Malee
{
    /// <summary>
    /// Class ReorderableArray.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.ICloneable" />
    /// <seealso cref="System.Collections.Generic.IList{T}" />
    /// <seealso cref="System.Collections.Generic.ICollection{T}" />
    /// <seealso cref="System.Collections.Generic.IEnumerable{T}" />
    [Serializable]
    public abstract class ReorderableArray<T> : ICloneable, IList<T>, ICollection<T>, IEnumerable<T>
    {
        [SerializeField]
        private List<T> array = new List<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderableArray{T}"/> class.
        /// </summary>
        public ReorderableArray()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReorderableArray{T}"/> class.
        /// </summary>
        /// <param name="length">The length.</param>
        public ReorderableArray(int length)
        {
            array = new List<T>(length);
        }

        /// <summary>
        /// Gets or sets the <see cref="T"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>T.</returns>
        public T this[int index]
        {
            get { return array[index]; }
            set { array[index] = value; }
        }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return array.Count; }
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
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return array.Count; }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>System.Object.</returns>
        public object Clone()
        {
            return new List<T>(array);
        }

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.</returns>
        public bool Contains(T value)
        {
            return array.Contains(value);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public int IndexOf(T value)
        {
            return array.IndexOf(value);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public void Insert(int index, T item)
        {
            array.Insert(index, item);
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            array.RemoveAt(index);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            array.Add(item);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            array.Clear();
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.array.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(T item)
        {
            return array.Remove(item);
        }

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns>T[].</returns>
        public T[] ToArray()
        {
            return array.ToArray();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>IEnumerator&lt;T&gt;.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return array.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return array.GetEnumerator();
        }
    }
}