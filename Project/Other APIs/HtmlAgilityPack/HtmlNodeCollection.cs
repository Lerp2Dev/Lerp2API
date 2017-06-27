namespace HtmlAgilityPack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Class HtmlNodeCollection.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IList{HtmlAgilityPack.HtmlNode}" />
    /// <seealso cref="System.Collections.Generic.ICollection{HtmlAgilityPack.HtmlNode}" />
    /// <seealso cref="System.Collections.Generic.IEnumerable{HtmlAgilityPack.HtmlNode}" />
    /// <seealso cref="System.Collections.IEnumerable" />
    public class HtmlNodeCollection : IList<HtmlNode>, ICollection<HtmlNode>, IEnumerable<HtmlNode>, IEnumerable
    {
        private readonly List<HtmlNode> _items = new List<HtmlNode>();
        private readonly HtmlNode _parentnode;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeCollection"/> class.
        /// </summary>
        /// <param name="parentnode">The parentnode.</param>
        public HtmlNodeCollection(HtmlNode parentnode)
        {
            _parentnode = parentnode;
        }

        /// <summary>
        /// Adds the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        public void Add(HtmlNode node)
        {
            _items.Add(node);
        }

        /// <summary>
        /// Appends the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <exception cref="InvalidProgramException">Unexpected error.</exception>
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

        /// <summary>
        /// Clears this instance.
        /// </summary>
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

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.</returns>
        public bool Contains(HtmlNode item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(HtmlNode[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Descendantses this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
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

        /// <summary>
        /// Descendantses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
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

        /// <summary>
        /// Elementses this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
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

        /// <summary>
        /// Elementses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
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

        /// <summary>
        /// Finds the first.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>HtmlNode.</returns>
        public HtmlNode FindFirst(string name)
        {
            return FindFirst(this, name);
        }

        /// <summary>
        /// Finds the first.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="name">The name.</param>
        /// <returns>HtmlNode.</returns>
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

        /// <summary>
        /// Gets the index of the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>System.Int32.</returns>
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

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>System.Int32.</returns>
        public int IndexOf(HtmlNode item)
        {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="node">The node.</param>
        /// <exception cref="InvalidProgramException">
        /// Unexpected error.
        /// or
        /// Unexpected error.
        /// </exception>
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

        /// <summary>
        /// Nodeses this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
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

        /// <summary>
        /// Prepends the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <exception cref="InvalidProgramException">Unexpected error.</exception>
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

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(HtmlNode item)
        {
            int index = _items.IndexOf(item);
            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Remove(int index)
        {
            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="InvalidProgramException">Unexpected error.</exception>
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

        /// <summary>
        /// Replaces the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="node">The node.</param>
        /// <exception cref="InvalidProgramException">
        /// Unexpected error.
        /// or
        /// Unexpected error.
        /// </exception>
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
        /// Gets the <see cref="HtmlNode"/> with the specified node name.
        /// </summary>
        /// <param name="nodeName">Name of the node.</param>
        /// <returns>HtmlNode.</returns>
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

        /// <summary>
        /// Gets or sets the <see cref="HtmlNode"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>HtmlNode.</returns>
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

        /// <summary>
        /// Gets the <see cref="System.Int32"/> with the specified node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="ArgumentOutOfRangeException">node;Node \"" + node.CloneNode(false).OuterHtml + "\" was not found in the collection</exception>
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