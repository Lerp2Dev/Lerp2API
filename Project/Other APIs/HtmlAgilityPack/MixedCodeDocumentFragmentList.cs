namespace HtmlAgilityPack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Class MixedCodeDocumentFragmentList.
    /// </summary>
    /// <seealso cref="System.Collections.IEnumerable" />
    public class MixedCodeDocumentFragmentList : IEnumerable
    {
        private MixedCodeDocument _doc;
        private IList<MixedCodeDocumentFragment> _items = new List<MixedCodeDocumentFragment>();

        internal MixedCodeDocumentFragmentList(MixedCodeDocument doc)
        {
            _doc = doc;
        }

        /// <summary>
        /// Appends the specified new fragment.
        /// </summary>
        /// <param name="newFragment">The new fragment.</param>
        /// <exception cref="ArgumentNullException">newFragment</exception>
        public void Append(MixedCodeDocumentFragment newFragment)
        {
            if (newFragment == null)
            {
                throw new ArgumentNullException("newFragment");
            }
            _items.Add(newFragment);
        }

        internal void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>MixedCodeDocumentFragmentEnumerator.</returns>
        public MixedCodeDocumentFragmentEnumerator GetEnumerator()
        {
            return new MixedCodeDocumentFragmentEnumerator(_items);
        }

        internal int GetFragmentIndex(MixedCodeDocumentFragment fragment)
        {
            if (fragment == null)
            {
                throw new ArgumentNullException("fragment");
            }
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] == fragment)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Prepends the specified new fragment.
        /// </summary>
        /// <param name="newFragment">The new fragment.</param>
        /// <exception cref="ArgumentNullException">newFragment</exception>
        public void Prepend(MixedCodeDocumentFragment newFragment)
        {
            if (newFragment == null)
            {
                throw new ArgumentNullException("newFragment");
            }
            _items.Insert(0, newFragment);
        }

        /// <summary>
        /// Removes the specified fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <exception cref="ArgumentNullException">fragment</exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void Remove(MixedCodeDocumentFragment fragment)
        {
            if (fragment == null)
            {
                throw new ArgumentNullException("fragment");
            }
            int fragmentIndex = GetFragmentIndex(fragment);
            if (fragmentIndex == -1)
            {
                throw new IndexOutOfRangeException();
            }
            RemoveAt(fragmentIndex);
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        public void RemoveAll()
        {
            _items.Clear();
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <value>The document.</value>
        public MixedCodeDocument Doc
        {
            get
            {
                return _doc;
            }
        }

        /// <summary>
        /// Gets the <see cref="MixedCodeDocumentFragment"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>MixedCodeDocumentFragment.</returns>
        public MixedCodeDocumentFragment this[int index]
        {
            get
            {
                return _items[index];
            }
        }

        /// <summary>
        /// Class MixedCodeDocumentFragmentEnumerator.
        /// </summary>
        /// <seealso cref="System.Collections.IEnumerator" />
        public class MixedCodeDocumentFragmentEnumerator : IEnumerator
        {
            private int _index;
            private IList<MixedCodeDocumentFragment> _items;

            internal MixedCodeDocumentFragmentEnumerator(IList<MixedCodeDocumentFragment> items)
            {
                _items = items;
                _index = -1;
            }

            /// <summary>
            /// Moves the next.
            /// </summary>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public bool MoveNext()
            {
                _index++;
                return (_index < _items.Count);
            }

            /// <summary>
            /// Resets this instance.
            /// </summary>
            public void Reset()
            {
                _index = -1;
            }

            /// <summary>
            /// Gets the current.
            /// </summary>
            /// <value>The current.</value>
            public MixedCodeDocumentFragment Current
            {
                get
                {
                    return _items[_index];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }
        }
    }
}