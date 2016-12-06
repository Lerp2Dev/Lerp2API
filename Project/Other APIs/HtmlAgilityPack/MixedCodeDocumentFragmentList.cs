namespace HtmlAgilityPack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class MixedCodeDocumentFragmentList : IEnumerable
    {
        private MixedCodeDocument _doc;
        private IList<MixedCodeDocumentFragment> _items = new List<MixedCodeDocumentFragment>();

        internal MixedCodeDocumentFragmentList(MixedCodeDocument doc)
        {
            _doc = doc;
        }

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

        public void Prepend(MixedCodeDocumentFragment newFragment)
        {
            if (newFragment == null)
            {
                throw new ArgumentNullException("newFragment");
            }
            _items.Insert(0, newFragment);
        }

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

        public void RemoveAll()
        {
            _items.Clear();
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public MixedCodeDocument Doc
        {
            get
            {
                return _doc;
            }
        }

        public MixedCodeDocumentFragment this[int index]
        {
            get
            {
                return _items[index];
            }
        }

        public class MixedCodeDocumentFragmentEnumerator : IEnumerator
        {
            private int _index;
            private IList<MixedCodeDocumentFragment> _items;

            internal MixedCodeDocumentFragmentEnumerator(IList<MixedCodeDocumentFragment> items)
            {
                _items = items;
                _index = -1;
            }

            public bool MoveNext()
            {
                _index++;
                return (_index < _items.Count);
            }

            public void Reset()
            {
                _index = -1;
            }

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