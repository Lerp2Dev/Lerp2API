namespace HtmlAgilityPack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Class HtmlAttributeCollection.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IList{HtmlAgilityPack.HtmlAttribute}" />
    /// <seealso cref="System.Collections.Generic.ICollection{HtmlAgilityPack.HtmlAttribute}" />
    /// <seealso cref="System.Collections.Generic.IEnumerable{HtmlAgilityPack.HtmlAttribute}" />
    /// <seealso cref="System.Collections.IEnumerable" />
    public class HtmlAttributeCollection : IList<HtmlAttribute>, ICollection<HtmlAttribute>, IEnumerable<HtmlAttribute>, IEnumerable
    {
        private HtmlNode _ownernode;
        internal Dictionary<string, HtmlAttribute> Hashitems = new Dictionary<string, HtmlAttribute>();
        private List<HtmlAttribute> items = new List<HtmlAttribute>();

        internal HtmlAttributeCollection(HtmlNode ownernode)
        {
            _ownernode = ownernode;
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(HtmlAttribute item)
        {
            Append(item);
        }

        /// <summary>
        /// Adds the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void Add(string name, string value)
        {
            Append(name, value);
        }

        /// <summary>
        /// Appends the specified new attribute.
        /// </summary>
        /// <param name="newAttribute">The new attribute.</param>
        /// <returns>HtmlAttribute.</returns>
        /// <exception cref="ArgumentNullException">newAttribute</exception>
        public HtmlAttribute Append(HtmlAttribute newAttribute)
        {
            if (newAttribute == null)
            {
                throw new ArgumentNullException("newAttribute");
            }
            Hashitems[newAttribute.Name] = newAttribute;
            newAttribute._ownernode = _ownernode;
            items.Add(newAttribute);
            _ownernode._innerchanged = true;
            _ownernode._outerchanged = true;
            return newAttribute;
        }

        /// <summary>
        /// Appends the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>HtmlAttribute.</returns>
        public HtmlAttribute Append(string name)
        {
            HtmlAttribute newAttribute = _ownernode._ownerdocument.CreateAttribute(name);
            return Append(newAttribute);
        }

        /// <summary>
        /// Appends the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>HtmlAttribute.</returns>
        public HtmlAttribute Append(string name, string value)
        {
            HtmlAttribute newAttribute = _ownernode._ownerdocument.CreateAttribute(name, value);
            return Append(newAttribute);
        }

        /// <summary>
        /// Attributeses the name of the with.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>IEnumerable&lt;HtmlAttribute&gt;.</returns>
        public IEnumerable<HtmlAttribute> AttributesWithName(string attributeName)
        {
            attributeName = attributeName.ToLower();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Name.Equals(attributeName))
                {
                    yield return items[i];
                }
            }
        }

        internal void Clear()
        {
            Hashitems.Clear();
            items.Clear();
        }

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
        public bool Contains(HtmlAttribute item)
        {
            return items.Contains(item);
        }

        /// <summary>
        /// Determines whether [contains] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if [contains] [the specified name]; otherwise, <c>false</c>.</returns>
        public bool Contains(string name)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Name.Equals(name.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(HtmlAttribute[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        internal int GetAttributeIndex(HtmlAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute");
            }
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] == attribute)
                {
                    return i;
                }
            }
            return -1;
        }

        internal int GetAttributeIndex(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            string str = name.ToLower();
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Name == str)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Int32.</returns>
        public int IndexOf(HtmlAttribute item)
        {
            return items.IndexOf(item);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="ArgumentNullException">item</exception>
        public void Insert(int index, HtmlAttribute item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            Hashitems[item.Name] = item;
            item._ownernode = _ownernode;
            items.Insert(index, item);
            _ownernode._innerchanged = true;
            _ownernode._outerchanged = true;
        }

        /// <summary>
        /// Prepends the specified new attribute.
        /// </summary>
        /// <param name="newAttribute">The new attribute.</param>
        /// <returns>HtmlAttribute.</returns>
        public HtmlAttribute Prepend(HtmlAttribute newAttribute)
        {
            Insert(0, newAttribute);
            return newAttribute;
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            foreach (HtmlAttribute attribute in items)
            {
                attribute.Remove();
            }
        }

        /// <summary>
        /// Removes the specified attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <exception cref="ArgumentNullException">attribute</exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void Remove(HtmlAttribute attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute");
            }
            int attributeIndex = GetAttributeIndex(attribute);
            if (attributeIndex == -1)
            {
                throw new IndexOutOfRangeException();
            }
            RemoveAt(attributeIndex);
        }

        /// <summary>
        /// Removes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">name</exception>
        public void Remove(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            string str = name.ToLower();
            for (int i = 0; i < items.Count; i++)
            {
                HtmlAttribute attribute = items[i];
                if (attribute.Name == str)
                {
                    RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        public void RemoveAll()
        {
            Hashitems.Clear();
            items.Clear();
            _ownernode._innerchanged = true;
            _ownernode._outerchanged = true;
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            HtmlAttribute attribute = items[index];
            Hashitems.Remove(attribute.Name);
            items.RemoveAt(index);
            _ownernode._innerchanged = true;
            _ownernode._outerchanged = true;
        }

        void ICollection<HtmlAttribute>.Clear()
        {
            items.Clear();
        }

        bool ICollection<HtmlAttribute>.Remove(HtmlAttribute item)
        {
            return items.Remove(item);
        }

        IEnumerator<HtmlAttribute> IEnumerable<HtmlAttribute>.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="HtmlAttribute"/> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>HtmlAttribute.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public HtmlAttribute this[string name]
        {
            get
            {
                HtmlAttribute attribute;
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }
                if (!Hashitems.TryGetValue(name.ToLower(), out attribute))
                {
                    return null;
                }
                return attribute;
            }
            set
            {
                Append(value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="HtmlAttribute"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>HtmlAttribute.</returns>
        public HtmlAttribute this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }
    }
}