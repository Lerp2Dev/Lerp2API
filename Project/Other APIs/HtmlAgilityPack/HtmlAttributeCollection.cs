namespace HtmlAgilityPack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class HtmlAttributeCollection : IList<HtmlAttribute>, ICollection<HtmlAttribute>, IEnumerable<HtmlAttribute>, IEnumerable
    {
        private HtmlNode _ownernode;
        internal Dictionary<string, HtmlAttribute> Hashitems = new Dictionary<string, HtmlAttribute>();
        private List<HtmlAttribute> items = new List<HtmlAttribute>();

        internal HtmlAttributeCollection(HtmlNode ownernode)
        {
            _ownernode = ownernode;
        }

        public void Add(HtmlAttribute item)
        {
            Append(item);
        }

        public void Add(string name, string value)
        {
            Append(name, value);
        }

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

        public HtmlAttribute Append(string name)
        {
            HtmlAttribute newAttribute = _ownernode._ownerdocument.CreateAttribute(name);
            return Append(newAttribute);
        }

        public HtmlAttribute Append(string name, string value)
        {
            HtmlAttribute newAttribute = _ownernode._ownerdocument.CreateAttribute(name, value);
            return Append(newAttribute);
        }

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

        public bool Contains(HtmlAttribute item)
        {
            return items.Contains(item);
        }

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

        public int IndexOf(HtmlAttribute item)
        {
            return items.IndexOf(item);
        }

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

        public HtmlAttribute Prepend(HtmlAttribute newAttribute)
        {
            Insert(0, newAttribute);
            return newAttribute;
        }

        public void Remove()
        {
            foreach (HtmlAttribute attribute in items)
            {
                attribute.Remove();
            }
        }

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

        public void RemoveAll()
        {
            Hashitems.Clear();
            items.Clear();
            _ownernode._innerchanged = true;
            _ownernode._outerchanged = true;
        }

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

        public int Count
        {
            get
            {
                return items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

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