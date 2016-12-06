namespace HtmlAgilityPack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class HtmlNodeCollection : IList<HtmlNode>, ICollection<HtmlNode>, IEnumerable<HtmlNode>, IEnumerable
    {
        private readonly List<HtmlNode> _items = new List<HtmlNode>();
        private readonly HtmlNode _parentnode;

        public HtmlNodeCollection(HtmlNode parentnode)
        {
            _parentnode = parentnode;
        }

        public void Add(HtmlNode node)
        {
            _items.Add(node);
        }

        public void Append(HtmlNode node)
        {
            HtmlNode node2 = null;
            if (_items.Count > 0)
            {
                node2 = _items[_items.Count - 1];
            }
            _items.Add(node);
            node._prevnode = node2;
            node._nextnode = null;
            node._parentnode = _parentnode;
            if (node2 != null)
            {
                if (node2 == node)
                {
                    throw new InvalidProgramException("Unexpected error.");
                }
                node2._nextnode = node;
            }
        }

        public void Clear()
        {
            foreach (HtmlNode node in _items)
            {
                node.ParentNode = null;
                node.NextSibling = null;
                node.PreviousSibling = null;
            }
            _items.Clear();
        }

        public bool Contains(HtmlNode item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(HtmlNode[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerable<HtmlNode> Descendants()
        {
            foreach (HtmlNode iteratorVariable0 in _items)
            {
                foreach (HtmlNode iteratorVariable1 in iteratorVariable0.Descendants())
                {
                    yield return iteratorVariable1;
                }
            }
        }

        public IEnumerable<HtmlNode> Descendants(string name)
        {
            foreach (HtmlNode iteratorVariable0 in _items)
            {
                foreach (HtmlNode iteratorVariable1 in iteratorVariable0.Descendants(name))
                {
                    yield return iteratorVariable1;
                }
            }
        }

        public IEnumerable<HtmlNode> Elements()
        {
            foreach (HtmlNode iteratorVariable0 in _items)
            {
                foreach (HtmlNode iteratorVariable1 in (IEnumerable<HtmlNode>)iteratorVariable0.ChildNodes)
                {
                    yield return iteratorVariable1;
                }
            }
        }

        public IEnumerable<HtmlNode> Elements(string name)
        {
            foreach (HtmlNode iteratorVariable0 in _items)
            {
                foreach (HtmlNode iteratorVariable1 in iteratorVariable0.Elements(name))
                {
                    yield return iteratorVariable1;
                }
            }
        }

        public HtmlNode FindFirst(string name)
        {
            return FindFirst(this, name);
        }

        public static HtmlNode FindFirst(HtmlNodeCollection items, string name)
        {
            foreach (HtmlNode node in (IEnumerable<HtmlNode>)items)
            {
                if (node.Name.ToLower().Contains(name))
                {
                    return node;
                }
                if (node.HasChildNodes)
                {
                    HtmlNode node2 = FindFirst(node.ChildNodes, name);
                    if (node2 != null)
                    {
                        return node2;
                    }
                }
            }
            return null;
        }

        public int GetNodeIndex(HtmlNode node)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (node == _items[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public int IndexOf(HtmlNode item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, HtmlNode node)
        {
            HtmlNode node2 = null;
            HtmlNode node3 = null;
            if (index > 0)
            {
                node3 = _items[index - 1];
            }
            if (index < _items.Count)
            {
                node2 = _items[index];
            }
            _items.Insert(index, node);
            if (node3 != null)
            {
                if (node == node3)
                {
                    throw new InvalidProgramException("Unexpected error.");
                }
                node3._nextnode = node;
            }
            if (node2 != null)
            {
                node2._prevnode = node;
            }
            node._prevnode = node3;
            if (node2 == node)
            {
                throw new InvalidProgramException("Unexpected error.");
            }
            node._nextnode = node2;
            node._parentnode = _parentnode;
        }

        public IEnumerable<HtmlNode> Nodes()
        {
            foreach (HtmlNode iteratorVariable0 in _items)
            {
                foreach (HtmlNode iteratorVariable1 in (IEnumerable<HtmlNode>)iteratorVariable0.ChildNodes)
                {
                    yield return iteratorVariable1;
                }
            }
        }

        public void Prepend(HtmlNode node)
        {
            HtmlNode node2 = null;
            if (_items.Count > 0)
            {
                node2 = _items[0];
            }
            _items.Insert(0, node);
            if (node == node2)
            {
                throw new InvalidProgramException("Unexpected error.");
            }
            node._nextnode = node2;
            node._prevnode = null;
            node._parentnode = _parentnode;
            if (node2 != null)
            {
                node2._prevnode = node;
            }
        }

        public bool Remove(HtmlNode item)
        {
            int index = _items.IndexOf(item);
            RemoveAt(index);
            return true;
        }

        public bool Remove(int index)
        {
            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            HtmlNode node = null;
            HtmlNode node2 = null;
            HtmlNode node3 = _items[index];
            if (index > 0)
            {
                node2 = _items[index - 1];
            }
            if (index < (_items.Count - 1))
            {
                node = _items[index + 1];
            }
            _items.RemoveAt(index);
            if (node2 != null)
            {
                if (node == node2)
                {
                    throw new InvalidProgramException("Unexpected error.");
                }
                node2._nextnode = node;
            }
            if (node != null)
            {
                node._prevnode = node2;
            }
            node3._prevnode = null;
            node3._nextnode = null;
            node3._parentnode = null;
        }

        public void Replace(int index, HtmlNode node)
        {
            HtmlNode node2 = null;
            HtmlNode node3 = null;
            HtmlNode node4 = _items[index];
            if (index > 0)
            {
                node3 = _items[index - 1];
            }
            if (index < (_items.Count - 1))
            {
                node2 = _items[index + 1];
            }
            _items[index] = node;
            if (node3 != null)
            {
                if (node == node3)
                {
                    throw new InvalidProgramException("Unexpected error.");
                }
                node3._nextnode = node;
            }
            if (node2 != null)
            {
                node2._prevnode = node;
            }
            node._prevnode = node3;
            if (node2 == node)
            {
                throw new InvalidProgramException("Unexpected error.");
            }
            node._nextnode = node2;
            node._parentnode = _parentnode;
            node4._prevnode = null;
            node4._nextnode = null;
            node4._parentnode = null;
        }

        IEnumerator<HtmlNode> IEnumerable<HtmlNode>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public HtmlNode this[string nodeName]
        {
            get
            {
                nodeName = nodeName.ToLower();
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].Name.Equals(nodeName))
                    {
                        return _items[i];
                    }
                }
                return null;
            }
        }

        public HtmlNode this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index] = value;
            }
        }

        public int this[HtmlNode node]
        {
            get
            {
                int nodeIndex = GetNodeIndex(node);
                if (nodeIndex == -1)
                {
                    throw new ArgumentOutOfRangeException("node", "Node \"" + node.CloneNode(false).OuterHtml + "\" was not found in the collection");
                }
                return nodeIndex;
            }
        }
    }
}