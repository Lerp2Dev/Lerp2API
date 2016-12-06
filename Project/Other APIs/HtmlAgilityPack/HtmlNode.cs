namespace HtmlAgilityPack
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using System.Xml.XPath;

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
        public static Dictionary<string, HtmlElementFlag> ElementsFlags = new Dictionary<string, HtmlElementFlag>();
        public static readonly string HtmlNodeTypeNameComment = "#comment";
        public static readonly string HtmlNodeTypeNameDocument = "#document";
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

        public HtmlNode Clone()
        {
            return CloneNode(true);
        }

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

        public HtmlNode CloneNode(string newName)
        {
            return CloneNode(newName, true);
        }

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

        public void CopyFrom(HtmlNode node)
        {
            CopyFrom(node, true);
        }

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

        public XPathNavigator CreateNavigator()
        {
            return new HtmlNodeNavigator(OwnerDocument, this);
        }

        public static HtmlNode CreateNode(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            return document.DocumentNode.FirstChild;
        }

        public XPathNavigator CreateRootNavigator()
        {
            return new HtmlNodeNavigator(OwnerDocument, OwnerDocument.DocumentNode);
        }

        public IEnumerable<HtmlAttribute> ChildAttributes(string name)
        {
            return Attributes.AttributesWithName(name);
        }

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

        [Obsolete("Use DescendantsAndSelf() instead, the results of this function will change in a future version")]
        public IEnumerable<HtmlNode> DescendantNodesAndSelf()
        {
            return DescendantsAndSelf();
        }

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

        public void Remove()
        {
            if (ParentNode != null)
            {
                ParentNode.ChildNodes.Remove(this);
            }
        }

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

        public string WriteContentTo()
        {
            StringWriter outText = new StringWriter();
            WriteContentTo(outText);
            outText.Flush();
            return outText.ToString();
        }

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

        public string WriteTo()
        {
            using (StringWriter writer = new StringWriter())
            {
                WriteTo(writer);
                writer.Flush();
                return writer.ToString();
            }
        }

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

        public bool Closed
        {
            get
            {
                return (_endnode != null);
            }
        }

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

        public string OriginalName
        {
            get
            {
                return _name;
            }
        }

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

        public int StreamPosition
        {
            get
            {
                return _streamposition;
            }
        }

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