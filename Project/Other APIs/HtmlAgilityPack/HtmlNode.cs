namespace HtmlAgilityPack
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;

    /// <summary>
    /// Class HtmlNode.
    /// </summary>
    /// <seealso cref="System.Xml.XPath.IXPathNavigable" />
    [DebuggerDisplay("Name: {OriginalName}}")]
    public class HtmlNode : IXPathNavigable
    {
        internal HtmlAttributeCollection _attributes;
        internal HtmlNodeCollection _childnodes;
        internal HtmlNode _endnode;
        internal bool _innerchanged;
        internal string _innerhtml;
        internal int _innerlength;
        internal int _innerstartindex;
        internal int _line;
        internal int _lineposition;
        private string _name;
        internal int _namelength;
        internal int _namestartindex;
        internal HtmlNode _nextnode;
        internal HtmlNodeType _nodetype;
        private string _optimizedName;
        internal bool _outerchanged;
        internal string _outerhtml;
        internal int _outerlength;
        internal int _outerstartindex;
        internal HtmlDocument _ownerdocument;
        internal HtmlNode _parentnode;
        internal HtmlNode _prevnode;
        internal HtmlNode _prevwithsamename;
        internal bool _starttag;
        internal int _streamposition;

        /// <summary>
        /// The elements flags
        /// </summary>
        public static Dictionary<string, HtmlElementFlag> ElementsFlags = new Dictionary<string, HtmlElementFlag>();

        /// <summary>
        /// The HTML node type name comment
        /// </summary>
        public static readonly string HtmlNodeTypeNameComment = "#comment";

        /// <summary>
        /// The HTML node type name document
        /// </summary>
        public static readonly string HtmlNodeTypeNameDocument = "#document";

        /// <summary>
        /// The HTML node type name text
        /// </summary>
        public static readonly string HtmlNodeTypeNameText = "#text";

        static HtmlNode()
        {
            ElementsFlags.Add("script", HtmlElementFlag.CData);
            ElementsFlags.Add("style", HtmlElementFlag.CData);
            ElementsFlags.Add("noxhtml", HtmlElementFlag.CData);
            ElementsFlags.Add("base", HtmlElementFlag.Empty);
            ElementsFlags.Add("link", HtmlElementFlag.Empty);
            ElementsFlags.Add("meta", HtmlElementFlag.Empty);
            ElementsFlags.Add("isindex", HtmlElementFlag.Empty);
            ElementsFlags.Add("hr", HtmlElementFlag.Empty);
            ElementsFlags.Add("col", HtmlElementFlag.Empty);
            ElementsFlags.Add("img", HtmlElementFlag.Empty);
            ElementsFlags.Add("param", HtmlElementFlag.Empty);
            ElementsFlags.Add("embed", HtmlElementFlag.Empty);
            ElementsFlags.Add("frame", HtmlElementFlag.Empty);
            ElementsFlags.Add("wbr", HtmlElementFlag.Empty);
            ElementsFlags.Add("bgsound", HtmlElementFlag.Empty);
            ElementsFlags.Add("spacer", HtmlElementFlag.Empty);
            ElementsFlags.Add("keygen", HtmlElementFlag.Empty);
            ElementsFlags.Add("area", HtmlElementFlag.Empty);
            ElementsFlags.Add("input", HtmlElementFlag.Empty);
            ElementsFlags.Add("basefont", HtmlElementFlag.Empty);
            ElementsFlags.Add("form", HtmlElementFlag.CanOverlap | HtmlElementFlag.Empty);
            ElementsFlags.Add("option", HtmlElementFlag.Empty);
            ElementsFlags.Add("br", HtmlElementFlag.Closed | HtmlElementFlag.Empty);
            ElementsFlags.Add("p", HtmlElementFlag.Closed | HtmlElementFlag.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNode"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="ownerdocument">The ownerdocument.</param>
        /// <param name="index">The index.</param>
        public HtmlNode(HtmlNodeType type, HtmlDocument ownerdocument, int index)
        {
            _nodetype = type;
            _ownerdocument = ownerdocument;
            _outerstartindex = index;
            switch (type)
            {
                case HtmlNodeType.Document:
                    Name = HtmlNodeTypeNameDocument;
                    _endnode = this;
                    break;

                case HtmlNodeType.Comment:
                    Name = HtmlNodeTypeNameComment;
                    _endnode = this;
                    break;

                case HtmlNodeType.Text:
                    Name = HtmlNodeTypeNameText;
                    _endnode = this;
                    break;
            }
            if (((_ownerdocument.Openednodes != null) && !Closed) && (-1 != index))
            {
                _ownerdocument.Openednodes.Add(index, this);
            }
            if (((-1 == index) && (type != HtmlNodeType.Comment)) && (type != HtmlNodeType.Text))
            {
                _outerchanged = true;
                _innerchanged = true;
            }
        }

        /// <summary>
        /// Ancestorses this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
        public IEnumerable<HtmlNode> Ancestors()
        {
            HtmlNode parentNode = ParentNode;
            while (true)
            {
                if (parentNode.ParentNode == null)
                {
                    yield break;
                }
                yield return parentNode.ParentNode;
                parentNode = parentNode.ParentNode;
            }
        }

        /// <summary>
        /// Ancestorses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
        public IEnumerable<HtmlNode> Ancestors(string name)
        {
            for (HtmlNode iteratorVariable0 = ParentNode; iteratorVariable0 != null; iteratorVariable0 = iteratorVariable0.ParentNode)
            {
                if (iteratorVariable0.Name == name)
                {
                    yield return iteratorVariable0;
                }
            }
        }

        /// <summary>
        /// Ancestorses the and self.
        /// </summary>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
        public IEnumerable<HtmlNode> AncestorsAndSelf()
        {
            HtmlNode parentNode = this;
            while (true)
            {
                if (parentNode == null)
                {
                    yield break;
                }
                yield return parentNode;
                parentNode = parentNode.ParentNode;
            }
        }

        /// <summary>
        /// Ancestorses the and self.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
        public IEnumerable<HtmlNode> AncestorsAndSelf(string name)
        {
            for (HtmlNode iteratorVariable0 = this; iteratorVariable0 != null; iteratorVariable0 = iteratorVariable0.ParentNode)
            {
                if (iteratorVariable0.Name == name)
                {
                    yield return iteratorVariable0;
                }
            }
        }

        /// <summary>
        /// Appends the child.
        /// </summary>
        /// <param name="newChild">The new child.</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentNullException">newChild</exception>
        public HtmlNode AppendChild(HtmlNode newChild)
        {
            if (newChild == null)
            {
                throw new ArgumentNullException("newChild");
            }
            ChildNodes.Append(newChild);
            _ownerdocument.SetIdForNode(newChild, newChild.GetId());
            _outerchanged = true;
            _innerchanged = true;
            return newChild;
        }

        /// <summary>
        /// Appends the children.
        /// </summary>
        /// <param name="newChildren">The new children.</param>
        /// <exception cref="ArgumentNullException">newChildren</exception>
        public void AppendChildren(HtmlNodeCollection newChildren)
        {
            if (newChildren == null)
            {
                throw new ArgumentNullException("newChildren");
            }
            foreach (HtmlNode node in (IEnumerable<HtmlNode>)newChildren)
            {
                AppendChild(node);
            }
        }

        /// <summary>
        /// Determines whether this instance [can overlap element] the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if this instance [can overlap element] the specified name; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public static bool CanOverlapElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (!ElementsFlags.ContainsKey(name.ToLower()))
            {
                return false;
            }
            HtmlElementFlag flag = ElementsFlags[name.ToLower()];
            return ((flag & HtmlElementFlag.CanOverlap) != 0);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>HtmlNode.</returns>
        public HtmlNode Clone()
        {
            return CloneNode(true);
        }

        /// <summary>
        /// Clones the node.
        /// </summary>
        /// <param name="deep">if set to <c>true</c> [deep].</param>
        /// <returns>HtmlNode.</returns>
        public HtmlNode CloneNode(bool deep)
        {
            HtmlNode node = _ownerdocument.CreateNode(_nodetype);
            node.Name = Name;
            switch (_nodetype)
            {
                case HtmlNodeType.Comment:
                    ((HtmlCommentNode)node).Comment = ((HtmlCommentNode)this).Comment;
                    return node;

                case HtmlNodeType.Text:
                    ((HtmlTextNode)node).Text = ((HtmlTextNode)this).Text;
                    return node;
            }
            if (HasAttributes)
            {
                foreach (HtmlAttribute attribute in (IEnumerable<HtmlAttribute>)_attributes)
                {
                    HtmlAttribute newAttribute = attribute.Clone();
                    node.Attributes.Append(newAttribute);
                }
            }
            if (HasClosingAttributes)
            {
                node._endnode = _endnode.CloneNode(false);
                foreach (HtmlAttribute attribute3 in (IEnumerable<HtmlAttribute>)_endnode._attributes)
                {
                    HtmlAttribute attribute4 = attribute3.Clone();
                    node._endnode._attributes.Append(attribute4);
                }
            }
            if (deep)
            {
                if (!HasChildNodes)
                {
                    return node;
                }
                foreach (HtmlNode node2 in (IEnumerable<HtmlNode>)_childnodes)
                {
                    HtmlNode newChild = node2.Clone();
                    node.AppendChild(newChild);
                }
            }
            return node;
        }

        /// <summary>
        /// Clones the node.
        /// </summary>
        /// <param name="newName">The new name.</param>
        /// <returns>HtmlNode.</returns>
        public HtmlNode CloneNode(string newName)
        {
            return CloneNode(newName, true);
        }

        /// <summary>
        /// Clones the node.
        /// </summary>
        /// <param name="newName">The new name.</param>
        /// <param name="deep">if set to <c>true</c> [deep].</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentNullException">newName</exception>
        public HtmlNode CloneNode(string newName, bool deep)
        {
            if (newName == null)
            {
                throw new ArgumentNullException("newName");
            }
            HtmlNode node = CloneNode(deep);
            node.Name = newName;
            return node;
        }

        internal void CloseNode(HtmlNode endnode)
        {
            if (!_ownerdocument.OptionAutoCloseOnEnd && (_childnodes != null))
            {
                foreach (HtmlNode node in (IEnumerable<HtmlNode>)_childnodes)
                {
                    if (!node.Closed)
                    {
                        HtmlNode node2 = null;
                        node2 = new HtmlNode(NodeType, _ownerdocument, -1)
                        {
                            _endnode = node2
                        };
                        node.CloseNode(node2);
                    }
                }
            }
            if (!Closed)
            {
                _endnode = endnode;
                if (_ownerdocument.Openednodes != null)
                {
                    _ownerdocument.Openednodes.Remove(_outerstartindex);
                }
                if (Utilities.GetDictionaryValueOrNull<string, HtmlNode>(_ownerdocument.Lastnodes, Name) == this)
                {
                    _ownerdocument.Lastnodes.Remove(Name);
                    _ownerdocument.UpdateLastParentNode();
                }
                if (endnode != this)
                {
                    _innerstartindex = _outerstartindex + _outerlength;
                    _innerlength = endnode._outerstartindex - _innerstartindex;
                    _outerlength = (endnode._outerstartindex + endnode._outerlength) - _outerstartindex;
                }
            }
        }

        /// <summary>
        /// Copies from.
        /// </summary>
        /// <param name="node">The node.</param>
        public void CopyFrom(HtmlNode node)
        {
            CopyFrom(node, true);
        }

        /// <summary>
        /// Copies from.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="deep">if set to <c>true</c> [deep].</param>
        /// <exception cref="ArgumentNullException">node</exception>
        public void CopyFrom(HtmlNode node, bool deep)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            Attributes.RemoveAll();
            if (node.HasAttributes)
            {
                foreach (HtmlAttribute attribute in (IEnumerable<HtmlAttribute>)node.Attributes)
                {
                    SetAttributeValue(attribute.Name, attribute.Value);
                }
            }
            if (!deep)
            {
                RemoveAllChildren();
                if (node.HasChildNodes)
                {
                    foreach (HtmlNode node2 in (IEnumerable<HtmlNode>)node.ChildNodes)
                    {
                        AppendChild(node2.CloneNode(true));
                    }
                }
            }
        }

        /// <summary>
        /// Creates the navigator.
        /// </summary>
        /// <returns>XPathNavigator.</returns>
        public XPathNavigator CreateNavigator()
        {
            return new HtmlNodeNavigator(OwnerDocument, this);
        }

        /// <summary>
        /// Creates the node.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns>HtmlNode.</returns>
        public static HtmlNode CreateNode(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            return document.DocumentNode.FirstChild;
        }

        /// <summary>
        /// Creates the root navigator.
        /// </summary>
        /// <returns>XPathNavigator.</returns>
        public XPathNavigator CreateRootNavigator()
        {
            return new HtmlNodeNavigator(OwnerDocument, OwnerDocument.DocumentNode);
        }

        /// <summary>
        /// Childs the attributes.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IEnumerable&lt;HtmlAttribute&gt;.</returns>
        public IEnumerable<HtmlAttribute> ChildAttributes(string name)
        {
            return Attributes.AttributesWithName(name);
        }

        /// <summary>
        /// Descendants the nodes.
        /// </summary>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
        [Obsolete("Use Descendants() instead, the results of this function will change in a future version")]
        public IEnumerable<HtmlNode> DescendantNodes()
        {
            foreach (HtmlNode iteratorVariable0 in (IEnumerable<HtmlNode>)ChildNodes)
            {
                yield return iteratorVariable0;
                foreach (HtmlNode iteratorVariable1 in iteratorVariable0.DescendantNodes())
                {
                    yield return iteratorVariable1;
                }
            }
        }

        /// <summary>
        /// Descendants the nodes and self.
        /// </summary>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
        [Obsolete("Use DescendantsAndSelf() instead, the results of this function will change in a future version")]
        public IEnumerable<HtmlNode> DescendantNodesAndSelf()
        {
            return DescendantsAndSelf();
        }

        /// <summary>
        /// Descendantses this instance.
        /// </summary>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
        public IEnumerable<HtmlNode> Descendants()
        {
            foreach (HtmlNode iteratorVariable0 in (IEnumerable<HtmlNode>)ChildNodes)
            {
                yield return iteratorVariable0;
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
            name = name.ToLowerInvariant();
            foreach (HtmlNode iteratorVariable0 in Descendants())
            {
                if (iteratorVariable0.Name.Equals(name))
                {
                    yield return iteratorVariable0;
                }
            }
        }

        /// <summary>
        /// Descendantses the and self.
        /// </summary>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
        public IEnumerable<HtmlNode> DescendantsAndSelf()
        {
            yield return this;
            foreach (HtmlNode iteratorVariable0 in Descendants())
            {
                HtmlNode iteratorVariable1 = iteratorVariable0;
                if (iteratorVariable1 != null)
                {
                    yield return iteratorVariable1;
                }
            }
        }

        /// <summary>
        /// Descendantses the and self.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
        public IEnumerable<HtmlNode> DescendantsAndSelf(string name)
        {
            yield return this;
            foreach (HtmlNode iteratorVariable0 in Descendants())
            {
                if (iteratorVariable0.Name == name)
                {
                    yield return iteratorVariable0;
                }
            }
        }

        /// <summary>
        /// Elements the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>HtmlNode.</returns>
        public HtmlNode Element(string name)
        {
            foreach (HtmlNode node in (IEnumerable<HtmlNode>)ChildNodes)
            {
                if (node.Name == name)
                {
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Elementses the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IEnumerable&lt;HtmlNode&gt;.</returns>
        public IEnumerable<HtmlNode> Elements(string name)
        {
            foreach (HtmlNode iteratorVariable0 in (IEnumerable<HtmlNode>)ChildNodes)
            {
                if (iteratorVariable0.Name == name)
                {
                    yield return iteratorVariable0;
                }
            }
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="def">if set to <c>true</c> [definition].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public bool GetAttributeValue(string name, bool def)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (!HasAttributes)
            {
                return def;
            }
            HtmlAttribute attribute = Attributes[name];
            if (attribute == null)
            {
                return def;
            }
            try
            {
                return Convert.ToBoolean(attribute.Value);
            }
            catch
            {
                return def;
            }
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="def">The definition.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public int GetAttributeValue(string name, int def)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (!HasAttributes)
            {
                return def;
            }
            HtmlAttribute attribute = Attributes[name];
            if (attribute == null)
            {
                return def;
            }
            try
            {
                return Convert.ToInt32(attribute.Value);
            }
            catch
            {
                return def;
            }
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="def">The definition.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public string GetAttributeValue(string name, string def)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (!HasAttributes)
            {
                return def;
            }
            HtmlAttribute attribute = Attributes[name];
            if (attribute == null)
            {
                return def;
            }
            return attribute.Value;
        }

        internal string GetId()
        {
            HtmlAttribute attribute = Attributes["id"];
            if (attribute != null)
            {
                return attribute.Value;
            }
            return string.Empty;
        }

        private string GetRelativeXpath()
        {
            if (ParentNode == null)
            {
                return Name;
            }
            if (NodeType == HtmlNodeType.Document)
            {
                return string.Empty;
            }
            int num = 1;
            foreach (HtmlNode node in (IEnumerable<HtmlNode>)ParentNode.ChildNodes)
            {
                if (!(node.Name != Name))
                {
                    if (node == this)
                    {
                        break;
                    }
                    num++;
                }
            }
            return string.Concat(new object[] { Name, "[", num, "]" });
        }

        internal static string GetXmlComment(HtmlCommentNode comment)
        {
            string str = comment.Comment;
            return str.Substring(4, str.Length - 7).Replace("--", " - -");
        }

        /// <summary>
        /// Inserts the after.
        /// </summary>
        /// <param name="newChild">The new child.</param>
        /// <param name="refChild">The reference child.</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentNullException">newChild</exception>
        /// <exception cref="ArgumentException"></exception>
        public HtmlNode InsertAfter(HtmlNode newChild, HtmlNode refChild)
        {
            if (newChild == null)
            {
                throw new ArgumentNullException("newChild");
            }
            if (refChild == null)
            {
                return PrependChild(newChild);
            }
            if (newChild != refChild)
            {
                int num = -1;
                if (_childnodes != null)
                {
                    num = _childnodes[refChild];
                }
                if (num == -1)
                {
                    throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
                }
                if (_childnodes != null)
                {
                    _childnodes.Insert(num + 1, newChild);
                }
                _ownerdocument.SetIdForNode(newChild, newChild.GetId());
                _outerchanged = true;
                _innerchanged = true;
            }
            return newChild;
        }

        /// <summary>
        /// Inserts the before.
        /// </summary>
        /// <param name="newChild">The new child.</param>
        /// <param name="refChild">The reference child.</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentNullException">newChild</exception>
        /// <exception cref="ArgumentException"></exception>
        public HtmlNode InsertBefore(HtmlNode newChild, HtmlNode refChild)
        {
            if (newChild == null)
            {
                throw new ArgumentNullException("newChild");
            }
            if (refChild == null)
            {
                return AppendChild(newChild);
            }
            if (newChild != refChild)
            {
                int index = -1;
                if (_childnodes != null)
                {
                    index = _childnodes[refChild];
                }
                if (index == -1)
                {
                    throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
                }
                if (_childnodes != null)
                {
                    _childnodes.Insert(index, newChild);
                }
                _ownerdocument.SetIdForNode(newChild, newChild.GetId());
                _outerchanged = true;
                _innerchanged = true;
            }
            return newChild;
        }

        /// <summary>
        /// Determines whether [is c data element] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if [is c data element] [the specified name]; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public static bool IsCDataElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (!ElementsFlags.ContainsKey(name.ToLower()))
            {
                return false;
            }
            HtmlElementFlag flag = ElementsFlags[name.ToLower()];
            return ((flag & HtmlElementFlag.CData) != 0);
        }

        /// <summary>
        /// Determines whether [is closed element] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if [is closed element] [the specified name]; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public static bool IsClosedElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (!ElementsFlags.ContainsKey(name.ToLower()))
            {
                return false;
            }
            HtmlElementFlag flag = ElementsFlags[name.ToLower()];
            return ((flag & HtmlElementFlag.Closed) != 0);
        }

        /// <summary>
        /// Determines whether [is empty element] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if [is empty element] [the specified name]; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public static bool IsEmptyElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length == 0)
            {
                return true;
            }
            if ('!' == name[0])
            {
                return true;
            }
            if ('?' == name[0])
            {
                return true;
            }
            if (!ElementsFlags.ContainsKey(name.ToLower()))
            {
                return false;
            }
            HtmlElementFlag flag = ElementsFlags[name.ToLower()];
            return ((flag & HtmlElementFlag.Empty) != 0);
        }

        /// <summary>
        /// Determines whether [is overlapped closing element] [the specified text].
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns><c>true</c> if [is overlapped closing element] [the specified text]; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">text</exception>
        public static bool IsOverlappedClosingElement(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            if (text.Length <= 4)
            {
                return false;
            }
            return ((((text[0] == '<') && (text[text.Length - 1] == '>')) && (text[1] == '/')) && CanOverlapElement(text.Substring(2, text.Length - 3)));
        }

        /// <summary>
        /// Prepends the child.
        /// </summary>
        /// <param name="newChild">The new child.</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentNullException">newChild</exception>
        public HtmlNode PrependChild(HtmlNode newChild)
        {
            if (newChild == null)
            {
                throw new ArgumentNullException("newChild");
            }
            ChildNodes.Prepend(newChild);
            _ownerdocument.SetIdForNode(newChild, newChild.GetId());
            _outerchanged = true;
            _innerchanged = true;
            return newChild;
        }

        /// <summary>
        /// Prepends the children.
        /// </summary>
        /// <param name="newChildren">The new children.</param>
        /// <exception cref="ArgumentNullException">newChildren</exception>
        public void PrependChildren(HtmlNodeCollection newChildren)
        {
            if (newChildren == null)
            {
                throw new ArgumentNullException("newChildren");
            }
            foreach (HtmlNode node in (IEnumerable<HtmlNode>)newChildren)
            {
                PrependChild(node);
            }
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            if (ParentNode != null)
            {
                ParentNode.ChildNodes.Remove(this);
            }
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        public void RemoveAll()
        {
            RemoveAllChildren();
            if (HasAttributes)
            {
                _attributes.Clear();
            }
            if (((_endnode != null) && (_endnode != this)) && (_endnode._attributes != null))
            {
                _endnode._attributes.Clear();
            }
            _outerchanged = true;
            _innerchanged = true;
        }

        /// <summary>
        /// Removes all children.
        /// </summary>
        public void RemoveAllChildren()
        {
            if (HasChildNodes)
            {
                if (_ownerdocument.OptionUseIdAttribute)
                {
                    foreach (HtmlNode node in (IEnumerable<HtmlNode>)_childnodes)
                    {
                        _ownerdocument.SetIdForNode(null, node.GetId());
                    }
                }
                _childnodes.Clear();
                _outerchanged = true;
                _innerchanged = true;
            }
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="oldChild">The old child.</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentNullException">oldChild</exception>
        /// <exception cref="ArgumentException"></exception>
        public HtmlNode RemoveChild(HtmlNode oldChild)
        {
            if (oldChild == null)
            {
                throw new ArgumentNullException("oldChild");
            }
            int index = -1;
            if (_childnodes != null)
            {
                index = _childnodes[oldChild];
            }
            if (index == -1)
            {
                throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
            }
            if (_childnodes != null)
            {
                _childnodes.Remove(index);
            }
            _ownerdocument.SetIdForNode(null, oldChild.GetId());
            _outerchanged = true;
            _innerchanged = true;
            return oldChild;
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="oldChild">The old child.</param>
        /// <param name="keepGrandChildren">if set to <c>true</c> [keep grand children].</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentNullException">oldChild</exception>
        public HtmlNode RemoveChild(HtmlNode oldChild, bool keepGrandChildren)
        {
            if (oldChild == null)
            {
                throw new ArgumentNullException("oldChild");
            }
            if ((oldChild._childnodes != null) && keepGrandChildren)
            {
                HtmlNode previousSibling = oldChild.PreviousSibling;
                foreach (HtmlNode node2 in (IEnumerable<HtmlNode>)oldChild._childnodes)
                {
                    InsertAfter(node2, previousSibling);
                }
            }
            RemoveChild(oldChild);
            _outerchanged = true;
            _innerchanged = true;
            return oldChild;
        }

        /// <summary>
        /// Replaces the child.
        /// </summary>
        /// <param name="newChild">The new child.</param>
        /// <param name="oldChild">The old child.</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentException"></exception>
        public HtmlNode ReplaceChild(HtmlNode newChild, HtmlNode oldChild)
        {
            if (newChild == null)
            {
                return RemoveChild(oldChild);
            }
            if (oldChild == null)
            {
                return AppendChild(newChild);
            }
            int index = -1;
            if (_childnodes != null)
            {
                index = _childnodes[oldChild];
            }
            if (index == -1)
            {
                throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
            }
            if (_childnodes != null)
            {
                _childnodes.Replace(index, newChild);
            }
            _ownerdocument.SetIdForNode(null, oldChild.GetId());
            _ownerdocument.SetIdForNode(newChild, newChild.GetId());
            _outerchanged = true;
            _innerchanged = true;
            return newChild;
        }

        /// <summary>
        /// Selects the nodes.
        /// </summary>
        /// <param name="xpath">The xpath.</param>
        /// <returns>HtmlNodeCollection.</returns>
        public HtmlNodeCollection SelectNodes(string xpath)
        {
            HtmlNodeCollection nodes = new HtmlNodeCollection(null);
            XPathNodeIterator iterator = new HtmlNodeNavigator(OwnerDocument, this).Select(xpath);
            while (iterator.MoveNext())
            {
                HtmlNodeNavigator current = (HtmlNodeNavigator)iterator.Current;
                nodes.Add(current.CurrentNode);
            }
            if (nodes.Count == 0)
            {
                return null;
            }
            return nodes;
        }

        /// <summary>
        /// Selects the single node.
        /// </summary>
        /// <param name="xpath">The xpath.</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentNullException">xpath</exception>
        public HtmlNode SelectSingleNode(string xpath)
        {
            if (xpath == null)
            {
                throw new ArgumentNullException("xpath");
            }
            XPathNodeIterator iterator = new HtmlNodeNavigator(OwnerDocument, this).Select(xpath);
            if (!iterator.MoveNext())
            {
                return null;
            }
            HtmlNodeNavigator current = (HtmlNodeNavigator)iterator.Current;
            return current.CurrentNode;
        }

        /// <summary>
        /// Sets the attribute value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>HtmlAttribute.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public HtmlAttribute SetAttributeValue(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            HtmlAttribute attribute = Attributes[name];
            if (attribute == null)
            {
                return Attributes.Append(_ownerdocument.CreateAttribute(name, value));
            }
            attribute.Value = value;
            return attribute;
        }

        internal void SetId(string id)
        {
            HtmlAttribute attribute = Attributes["id"] ?? _ownerdocument.CreateAttribute("id");
            attribute.Value = id;
            _ownerdocument.SetIdForNode(this, attribute.Value);
            _outerchanged = true;
        }

        internal void WriteAttribute(TextWriter outText, HtmlAttribute att)
        {
            string originalName;
            string str2 = (att.QuoteType == AttributeValueQuote.DoubleQuote) ? "\"" : "'";
            if (_ownerdocument.OptionOutputAsXml)
            {
                originalName = _ownerdocument.OptionOutputUpperCase ? att.XmlName.ToUpper() : att.XmlName;
                if (_ownerdocument.OptionOutputOriginalCase)
                {
                    originalName = att.OriginalName;
                }
                outText.Write(" " + originalName + "=" + str2 + HtmlDocument.HtmlEncode(att.XmlValue) + str2);
            }
            else
            {
                originalName = _ownerdocument.OptionOutputUpperCase ? att.Name.ToUpper() : att.Name;
                if ((((att.Name.Length >= 4) && (att.Name[0] == '<')) && ((att.Name[1] == '%') && (att.Name[att.Name.Length - 1] == '>'))) && (att.Name[att.Name.Length - 2] == '%'))
                {
                    outText.Write(" " + originalName);
                }
                else if (_ownerdocument.OptionOutputOptimizeAttributeValues)
                {
                    if (att.Value.IndexOfAny(new char[] { '\n', '\r', '\t', ' ' }) < 0)
                    {
                        outText.Write(" " + originalName + "=" + att.Value);
                    }
                    else
                    {
                        outText.Write(" " + originalName + "=" + str2 + att.Value + str2);
                    }
                }
                else
                {
                    outText.Write(" " + originalName + "=" + str2 + att.Value + str2);
                }
            }
        }

        internal void WriteAttributes(TextWriter outText, bool closing)
        {
            if (_ownerdocument.OptionOutputAsXml)
            {
                if (_attributes != null)
                {
                    foreach (HtmlAttribute attribute in _attributes.Hashitems.Values)
                    {
                        WriteAttribute(outText, attribute);
                    }
                }
            }
            else if (!closing)
            {
                if (_attributes != null)
                {
                    foreach (HtmlAttribute attribute2 in (IEnumerable<HtmlAttribute>)_attributes)
                    {
                        WriteAttribute(outText, attribute2);
                    }
                }
                if (_ownerdocument.OptionAddDebuggingAttributes)
                {
                    WriteAttribute(outText, _ownerdocument.CreateAttribute("_closed", Closed.ToString()));
                    WriteAttribute(outText, _ownerdocument.CreateAttribute("_children", ChildNodes.Count.ToString()));
                    int num = 0;
                    foreach (HtmlNode node in (IEnumerable<HtmlNode>)ChildNodes)
                    {
                        WriteAttribute(outText, _ownerdocument.CreateAttribute("_child_" + num, node.Name));
                        num++;
                    }
                }
            }
            else if (((_endnode != null) && (_endnode._attributes != null)) && (_endnode != this))
            {
                foreach (HtmlAttribute attribute3 in (IEnumerable<HtmlAttribute>)_endnode._attributes)
                {
                    WriteAttribute(outText, attribute3);
                }
                if (_ownerdocument.OptionAddDebuggingAttributes)
                {
                    WriteAttribute(outText, _ownerdocument.CreateAttribute("_closed", Closed.ToString()));
                    WriteAttribute(outText, _ownerdocument.CreateAttribute("_children", ChildNodes.Count.ToString()));
                }
            }
        }

        internal static void WriteAttributes(XmlWriter writer, HtmlNode node)
        {
            if (node.HasAttributes)
            {
                foreach (HtmlAttribute attribute in node.Attributes.Hashitems.Values)
                {
                    writer.WriteAttributeString(attribute.XmlName, attribute.Value);
                }
            }
        }

        /// <summary>
        /// Writes the content to.
        /// </summary>
        /// <returns>System.String.</returns>
        public string WriteContentTo()
        {
            StringWriter outText = new StringWriter();
            WriteContentTo(outText);
            outText.Flush();
            return outText.ToString();
        }

        /// <summary>
        /// Writes the content to.
        /// </summary>
        /// <param name="outText">The out text.</param>
        public void WriteContentTo(TextWriter outText)
        {
            if (_childnodes != null)
            {
                foreach (HtmlNode node in (IEnumerable<HtmlNode>)_childnodes)
                {
                    node.WriteTo(outText);
                }
            }
        }

        /// <summary>
        /// Writes to.
        /// </summary>
        /// <returns>System.String.</returns>
        public string WriteTo()
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer);
                writer.Flush();
                return writer.ToString();
            }
        }

        /// <summary>
        /// Writes to.
        /// </summary>
        /// <param name="outText">The out text.</param>
        public void WriteTo(TextWriter outText)
        {
            string comment;
            switch (_nodetype)
            {
                case HtmlNodeType.Document:
                    {
                        if (!_ownerdocument.OptionOutputAsXml)
                        {
                            break;
                        }
                        outText.Write("<?xml version=\"1.0\" encoding=\"" + _ownerdocument.GetOutEncoding().BodyName + "\"?>");
                        if (!_ownerdocument.DocumentNode.HasChildNodes)
                        {
                            break;
                        }
                        int count = _ownerdocument.DocumentNode._childnodes.Count;
                        if (count <= 0)
                        {
                            break;
                        }
                        if (_ownerdocument.GetXmlDeclaration() != null)
                        {
                            count--;
                        }
                        if (count <= 1)
                        {
                            break;
                        }
                        if (_ownerdocument.OptionOutputUpperCase)
                        {
                            outText.Write("<SPAN>");
                            WriteContentTo(outText);
                            outText.Write("</SPAN>");
                            return;
                        }
                        outText.Write("<span>");
                        WriteContentTo(outText);
                        outText.Write("</span>");
                        return;
                    }
                case HtmlNodeType.Element:
                    {
                        string name = _ownerdocument.OptionOutputUpperCase ? Name.ToUpper() : Name;
                        if (_ownerdocument.OptionOutputOriginalCase)
                        {
                            name = OriginalName;
                        }
                        if (_ownerdocument.OptionOutputAsXml)
                        {
                            if (name.Length <= 0)
                            {
                                return;
                            }
                            if (name[0] == '?')
                            {
                                return;
                            }
                            if (name.Trim().Length == 0)
                            {
                                return;
                            }
                            name = HtmlDocument.GetXmlName(name);
                        }
                        outText.Write("<" + name);
                        WriteAttributes(outText, false);
                        if (!HasChildNodes)
                        {
                            if (IsEmptyElement(Name))
                            {
                                if (_ownerdocument.OptionWriteEmptyNodes || _ownerdocument.OptionOutputAsXml)
                                {
                                    outText.Write(" />");
                                    return;
                                }
                                if ((Name.Length > 0) && (Name[0] == '?'))
                                {
                                    outText.Write("?");
                                }
                                outText.Write(">");
                                return;
                            }
                            outText.Write("></" + name + ">");
                            return;
                        }
                        outText.Write(">");
                        bool flag = false;
                        if (_ownerdocument.OptionOutputAsXml && IsCDataElement(Name))
                        {
                            flag = true;
                            outText.Write("\r\n//<![CDATA[\r\n");
                        }
                        if (flag)
                        {
                            if (HasChildNodes)
                            {
                                ChildNodes[0].WriteTo(outText);
                            }
                            outText.Write("\r\n//]]>//\r\n");
                        }
                        else
                        {
                            WriteContentTo(outText);
                        }
                        outText.Write("</" + name);
                        if (!_ownerdocument.OptionOutputAsXml)
                        {
                            WriteAttributes(outText, true);
                        }
                        outText.Write(">");
                        return;
                    }
                case HtmlNodeType.Comment:
                    comment = ((HtmlCommentNode)this).Comment;
                    if (!_ownerdocument.OptionOutputAsXml)
                    {
                        outText.Write(comment);
                        return;
                    }
                    outText.Write("<!--" + GetXmlComment((HtmlCommentNode)this) + " -->");
                    return;

                case HtmlNodeType.Text:
                    comment = ((HtmlTextNode)this).Text;
                    outText.Write(_ownerdocument.OptionOutputAsXml ? HtmlDocument.HtmlEncode(comment) : comment);
                    return;

                default:
                    return;
            }
            WriteContentTo(outText);
        }

        /// <summary>
        /// Writes to.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void WriteTo(XmlWriter writer)
        {
            switch (_nodetype)
            {
                case HtmlNodeType.Document:
                    writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"" + _ownerdocument.GetOutEncoding().BodyName + "\"");
                    if (HasChildNodes)
                    {
                        foreach (HtmlNode node in (IEnumerable<HtmlNode>)ChildNodes)
                        {
                            node.WriteTo(writer);
                        }
                    }
                    break;

                case HtmlNodeType.Element:
                    {
                        string localName = _ownerdocument.OptionOutputUpperCase ? Name.ToUpper() : Name;
                        if (_ownerdocument.OptionOutputOriginalCase)
                        {
                            localName = OriginalName;
                        }
                        writer.WriteStartElement(localName);
                        WriteAttributes(writer, this);
                        if (HasChildNodes)
                        {
                            foreach (HtmlNode node2 in (IEnumerable<HtmlNode>)ChildNodes)
                            {
                                node2.WriteTo(writer);
                            }
                        }
                        writer.WriteEndElement();
                        break;
                    }
                case HtmlNodeType.Comment:
                    writer.WriteComment(GetXmlComment((HtmlCommentNode)this));
                    return;

                case HtmlNodeType.Text:
                    {
                        string text = ((HtmlTextNode)this).Text;
                        writer.WriteString(text);
                        return;
                    }
                default:
                    return;
            }
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        public HtmlAttributeCollection Attributes
        {
            get
            {
                if (!HasAttributes)
                {
                    _attributes = new HtmlAttributeCollection(this);
                }
                return _attributes;
            }
            internal set
            {
                _attributes = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="HtmlNode"/> is closed.
        /// </summary>
        /// <value><c>true</c> if closed; otherwise, <c>false</c>.</value>
        public bool Closed
        {
            get
            {
                return (_endnode != null);
            }
        }

        /// <summary>
        /// Gets the closing attributes.
        /// </summary>
        /// <value>The closing attributes.</value>
        public HtmlAttributeCollection ClosingAttributes
        {
            get
            {
                if (HasClosingAttributes)
                {
                    return _endnode.Attributes;
                }
                return new HtmlAttributeCollection(this);
            }
        }

        /// <summary>
        /// Gets the child nodes.
        /// </summary>
        /// <value>The child nodes.</value>
        public HtmlNodeCollection ChildNodes
        {
            get
            {
                return (_childnodes ?? (_childnodes = new HtmlNodeCollection(this)));
            }
            internal set
            {
                _childnodes = value;
            }
        }

        internal HtmlNode EndNode
        {
            get
            {
                return _endnode;
            }
        }

        /// <summary>
        /// Gets the first child.
        /// </summary>
        /// <value>The first child.</value>
        public HtmlNode FirstChild
        {
            get
            {
                if (HasChildNodes)
                {
                    return _childnodes[0];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has attributes.
        /// </summary>
        /// <value><c>true</c> if this instance has attributes; otherwise, <c>false</c>.</value>
        public bool HasAttributes
        {
            get
            {
                if (_attributes == null)
                {
                    return false;
                }
                if (_attributes.Count <= 0)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has closing attributes.
        /// </summary>
        /// <value><c>true</c> if this instance has closing attributes; otherwise, <c>false</c>.</value>
        public bool HasClosingAttributes
        {
            get
            {
                if ((_endnode == null) || (_endnode == this))
                {
                    return false;
                }
                if (_endnode._attributes == null)
                {
                    return false;
                }
                if (_endnode._attributes.Count <= 0)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has child nodes.
        /// </summary>
        /// <value><c>true</c> if this instance has child nodes; otherwise, <c>false</c>.</value>
        public bool HasChildNodes
        {
            get
            {
                if (_childnodes == null)
                {
                    return false;
                }
                if (_childnodes.Count <= 0)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        /// <exception cref="Exception">
        /// </exception>
        /// <exception cref="ArgumentNullException">value</exception>
        public string Id
        {
            get
            {
                if (_ownerdocument.Nodesid == null)
                {
                    throw new Exception(HtmlDocument.HtmlExceptionUseIdAttributeFalse);
                }
                return GetId();
            }
            set
            {
                if (_ownerdocument.Nodesid == null)
                {
                    throw new Exception(HtmlDocument.HtmlExceptionUseIdAttributeFalse);
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                SetId(value);
            }
        }

        /// <summary>
        /// Gets or sets the inner HTML.
        /// </summary>
        /// <value>The inner HTML.</value>
        public virtual string InnerHtml
        {
            get
            {
                if (_innerchanged)
                {
                    _innerhtml = WriteContentTo();
                    _innerchanged = false;
                    return _innerhtml;
                }
                if (_innerhtml != null)
                {
                    return _innerhtml;
                }
                if (_innerstartindex < 0)
                {
                    return string.Empty;
                }
                return _ownerdocument.Text.Substring(_innerstartindex, _innerlength);
            }
            set
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(value);
                RemoveAllChildren();
                AppendChildren(document.DocumentNode.ChildNodes);
            }
        }

        /// <summary>
        /// Gets the inner text.
        /// </summary>
        /// <value>The inner text.</value>
        public virtual string InnerText
        {
            get
            {
                if (_nodetype == HtmlNodeType.Text)
                {
                    return ((HtmlTextNode)this).Text;
                }
                if (_nodetype == HtmlNodeType.Comment)
                {
                    return ((HtmlCommentNode)this).Comment;
                }
                if (!HasChildNodes)
                {
                    return string.Empty;
                }
                string str = null;
                foreach (HtmlNode node in (IEnumerable<HtmlNode>)ChildNodes)
                {
                    str = str + node.InnerText;
                }
                return str;
            }
        }

        /// <summary>
        /// Gets the last child.
        /// </summary>
        /// <value>The last child.</value>
        public HtmlNode LastChild
        {
            get
            {
                if (HasChildNodes)
                {
                    return _childnodes[_childnodes.Count - 1];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <value>The line.</value>
        public int Line
        {
            get
            {
                return _line;
            }
            internal set
            {
                _line = value;
            }
        }

        /// <summary>
        /// Gets the line position.
        /// </summary>
        /// <value>The line position.</value>
        public int LinePosition
        {
            get
            {
                return _lineposition;
            }
            internal set
            {
                _lineposition = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                if (_optimizedName == null)
                {
                    if (_name == null)
                    {
                        Name = _ownerdocument.Text.Substring(_namestartindex, _namelength);
                    }
                    if (_name == null)
                    {
                        _optimizedName = string.Empty;
                    }
                    else
                    {
                        _optimizedName = _name.ToLower();
                    }
                }
                return _optimizedName;
            }
            set
            {
                _name = value;
                _optimizedName = null;
            }
        }

        /// <summary>
        /// Gets the next sibling.
        /// </summary>
        /// <value>The next sibling.</value>
        public HtmlNode NextSibling
        {
            get
            {
                return _nextnode;
            }
            internal set
            {
                _nextnode = value;
            }
        }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        public HtmlNodeType NodeType
        {
            get
            {
                return _nodetype;
            }
            internal set
            {
                _nodetype = value;
            }
        }

        /// <summary>
        /// Gets the name of the original.
        /// </summary>
        /// <value>The name of the original.</value>
        public string OriginalName
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Gets the outer HTML.
        /// </summary>
        /// <value>The outer HTML.</value>
        public virtual string OuterHtml
        {
            get
            {
                if (_outerchanged)
                {
                    _outerhtml = WriteTo();
                    _outerchanged = false;
                    return _outerhtml;
                }
                if (_outerhtml != null)
                {
                    return _outerhtml;
                }
                if (_outerstartindex < 0)
                {
                    return string.Empty;
                }
                return _ownerdocument.Text.Substring(_outerstartindex, _outerlength);
            }
        }

        /// <summary>
        /// Gets the owner document.
        /// </summary>
        /// <value>The owner document.</value>
        public HtmlDocument OwnerDocument
        {
            get
            {
                return _ownerdocument;
            }
            internal set
            {
                _ownerdocument = value;
            }
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        public HtmlNode ParentNode
        {
            get
            {
                return _parentnode;
            }
            internal set
            {
                _parentnode = value;
            }
        }

        /// <summary>
        /// Gets the previous sibling.
        /// </summary>
        /// <value>The previous sibling.</value>
        public HtmlNode PreviousSibling
        {
            get
            {
                return _prevnode;
            }
            internal set
            {
                _prevnode = value;
            }
        }

        /// <summary>
        /// Gets the stream position.
        /// </summary>
        /// <value>The stream position.</value>
        public int StreamPosition
        {
            get
            {
                return _streamposition;
            }
        }

        /// <summary>
        /// Gets the x path.
        /// </summary>
        /// <value>The x path.</value>
        public string XPath
        {
            get
            {
                string str = ((ParentNode == null) || (ParentNode.NodeType == HtmlNodeType.Document)) ? "/" : (ParentNode.XPath + "/");
                return (str + GetRelativeXpath());
            }
        }
    }
}