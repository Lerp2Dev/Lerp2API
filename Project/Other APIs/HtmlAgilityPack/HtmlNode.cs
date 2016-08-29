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
            this._nodetype = type;
            this._ownerdocument = ownerdocument;
            this._outerstartindex = index;
            switch (type)
            {
                case HtmlNodeType.Document:
                    this.Name = HtmlNodeTypeNameDocument;
                    this._endnode = this;
                    break;

                case HtmlNodeType.Comment:
                    this.Name = HtmlNodeTypeNameComment;
                    this._endnode = this;
                    break;

                case HtmlNodeType.Text:
                    this.Name = HtmlNodeTypeNameText;
                    this._endnode = this;
                    break;
            }
            if (((this._ownerdocument.Openednodes != null) && !this.Closed) && (-1 != index))
            {
                this._ownerdocument.Openednodes.Add(index, this);
            }
            if (((-1 == index) && (type != HtmlNodeType.Comment)) && (type != HtmlNodeType.Text))
            {
                this._outerchanged = true;
                this._innerchanged = true;
            }
        }

        public IEnumerable<HtmlNode> Ancestors()
        {
            HtmlNode parentNode = this.ParentNode;
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
            for (HtmlNode iteratorVariable0 = this.ParentNode; iteratorVariable0 != null; iteratorVariable0 = iteratorVariable0.ParentNode)
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
            this.ChildNodes.Append(newChild);
            this._ownerdocument.SetIdForNode(newChild, newChild.GetId());
            this._outerchanged = true;
            this._innerchanged = true;
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
                this.AppendChild(node);
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
            return this.CloneNode(true);
        }

        public HtmlNode CloneNode(bool deep)
        {
            HtmlNode node = this._ownerdocument.CreateNode(this._nodetype);
            node.Name = this.Name;
            switch (this._nodetype)
            {
                case HtmlNodeType.Comment:
                    ((HtmlCommentNode)node).Comment = ((HtmlCommentNode)this).Comment;
                    return node;

                case HtmlNodeType.Text:
                    ((HtmlTextNode)node).Text = ((HtmlTextNode)this).Text;
                    return node;
            }
            if (this.HasAttributes)
            {
                foreach (HtmlAttribute attribute in (IEnumerable<HtmlAttribute>)this._attributes)
                {
                    HtmlAttribute newAttribute = attribute.Clone();
                    node.Attributes.Append(newAttribute);
                }
            }
            if (this.HasClosingAttributes)
            {
                node._endnode = this._endnode.CloneNode(false);
                foreach (HtmlAttribute attribute3 in (IEnumerable<HtmlAttribute>)this._endnode._attributes)
                {
                    HtmlAttribute attribute4 = attribute3.Clone();
                    node._endnode._attributes.Append(attribute4);
                }
            }
            if (deep)
            {
                if (!this.HasChildNodes)
                {
                    return node;
                }
                foreach (HtmlNode node2 in (IEnumerable<HtmlNode>)this._childnodes)
                {
                    HtmlNode newChild = node2.Clone();
                    node.AppendChild(newChild);
                }
            }
            return node;
        }

        public HtmlNode CloneNode(string newName)
        {
            return this.CloneNode(newName, true);
        }

        public HtmlNode CloneNode(string newName, bool deep)
        {
            if (newName == null)
            {
                throw new ArgumentNullException("newName");
            }
            HtmlNode node = this.CloneNode(deep);
            node.Name = newName;
            return node;
        }

        internal void CloseNode(HtmlNode endnode)
        {
            if (!this._ownerdocument.OptionAutoCloseOnEnd && (this._childnodes != null))
            {
                foreach (HtmlNode node in (IEnumerable<HtmlNode>)this._childnodes)
                {
                    if (!node.Closed)
                    {
                        HtmlNode node2 = null;
                        node2 = new HtmlNode(this.NodeType, this._ownerdocument, -1)
                        {
                            _endnode = node2
                        };
                        node.CloseNode(node2);
                    }
                }
            }
            if (!this.Closed)
            {
                this._endnode = endnode;
                if (this._ownerdocument.Openednodes != null)
                {
                    this._ownerdocument.Openednodes.Remove(this._outerstartindex);
                }
                if (Utilities.GetDictionaryValueOrNull<string, HtmlNode>(this._ownerdocument.Lastnodes, this.Name) == this)
                {
                    this._ownerdocument.Lastnodes.Remove(this.Name);
                    this._ownerdocument.UpdateLastParentNode();
                }
                if (endnode != this)
                {
                    this._innerstartindex = this._outerstartindex + this._outerlength;
                    this._innerlength = endnode._outerstartindex - this._innerstartindex;
                    this._outerlength = (endnode._outerstartindex + endnode._outerlength) - this._outerstartindex;
                }
            }
        }

        public void CopyFrom(HtmlNode node)
        {
            this.CopyFrom(node, true);
        }

        public void CopyFrom(HtmlNode node, bool deep)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            this.Attributes.RemoveAll();
            if (node.HasAttributes)
            {
                foreach (HtmlAttribute attribute in (IEnumerable<HtmlAttribute>)node.Attributes)
                {
                    this.SetAttributeValue(attribute.Name, attribute.Value);
                }
            }
            if (!deep)
            {
                this.RemoveAllChildren();
                if (node.HasChildNodes)
                {
                    foreach (HtmlNode node2 in (IEnumerable<HtmlNode>)node.ChildNodes)
                    {
                        this.AppendChild(node2.CloneNode(true));
                    }
                }
            }
        }

        public XPathNavigator CreateNavigator()
        {
            return new HtmlNodeNavigator(this.OwnerDocument, this);
        }

        public static HtmlNode CreateNode(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);
            return document.DocumentNode.FirstChild;
        }

        public XPathNavigator CreateRootNavigator()
        {
            return new HtmlNodeNavigator(this.OwnerDocument, this.OwnerDocument.DocumentNode);
        }

        public IEnumerable<HtmlAttribute> ChildAttributes(string name)
        {
            return this.Attributes.AttributesWithName(name);
        }

        [Obsolete("Use Descendants() instead, the results of this function will change in a future version")]
        public IEnumerable<HtmlNode> DescendantNodes()
        {
            foreach (HtmlNode iteratorVariable0 in (IEnumerable<HtmlNode>)this.ChildNodes)
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
            return this.DescendantsAndSelf();
        }

        public IEnumerable<HtmlNode> Descendants()
        {
            foreach (HtmlNode iteratorVariable0 in (IEnumerable<HtmlNode>)this.ChildNodes)
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
            foreach (HtmlNode iteratorVariable0 in this.Descendants())
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
            foreach (HtmlNode iteratorVariable0 in this.Descendants())
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
            foreach (HtmlNode iteratorVariable0 in this.Descendants())
            {
                if (iteratorVariable0.Name == name)
                {
                    yield return iteratorVariable0;
                }
            }
        }

        public HtmlNode Element(string name)
        {
            foreach (HtmlNode node in (IEnumerable<HtmlNode>)this.ChildNodes)
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
            foreach (HtmlNode iteratorVariable0 in (IEnumerable<HtmlNode>)this.ChildNodes)
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
            if (!this.HasAttributes)
            {
                return def;
            }
            HtmlAttribute attribute = this.Attributes[name];
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
            if (!this.HasAttributes)
            {
                return def;
            }
            HtmlAttribute attribute = this.Attributes[name];
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
            if (!this.HasAttributes)
            {
                return def;
            }
            HtmlAttribute attribute = this.Attributes[name];
            if (attribute == null)
            {
                return def;
            }
            return attribute.Value;
        }

        internal string GetId()
        {
            HtmlAttribute attribute = this.Attributes["id"];
            if (attribute != null)
            {
                return attribute.Value;
            }
            return string.Empty;
        }

        private string GetRelativeXpath()
        {
            if (this.ParentNode == null)
            {
                return this.Name;
            }
            if (this.NodeType == HtmlNodeType.Document)
            {
                return string.Empty;
            }
            int num = 1;
            foreach (HtmlNode node in (IEnumerable<HtmlNode>)this.ParentNode.ChildNodes)
            {
                if (!(node.Name != this.Name))
                {
                    if (node == this)
                    {
                        break;
                    }
                    num++;
                }
            }
            return string.Concat(new object[] { this.Name, "[", num, "]" });
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
                return this.PrependChild(newChild);
            }
            if (newChild != refChild)
            {
                int num = -1;
                if (this._childnodes != null)
                {
                    num = this._childnodes[refChild];
                }
                if (num == -1)
                {
                    throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
                }
                if (this._childnodes != null)
                {
                    this._childnodes.Insert(num + 1, newChild);
                }
                this._ownerdocument.SetIdForNode(newChild, newChild.GetId());
                this._outerchanged = true;
                this._innerchanged = true;
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
                return this.AppendChild(newChild);
            }
            if (newChild != refChild)
            {
                int index = -1;
                if (this._childnodes != null)
                {
                    index = this._childnodes[refChild];
                }
                if (index == -1)
                {
                    throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
                }
                if (this._childnodes != null)
                {
                    this._childnodes.Insert(index, newChild);
                }
                this._ownerdocument.SetIdForNode(newChild, newChild.GetId());
                this._outerchanged = true;
                this._innerchanged = true;
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
            this.ChildNodes.Prepend(newChild);
            this._ownerdocument.SetIdForNode(newChild, newChild.GetId());
            this._outerchanged = true;
            this._innerchanged = true;
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
                this.PrependChild(node);
            }
        }

        public void Remove()
        {
            if (this.ParentNode != null)
            {
                this.ParentNode.ChildNodes.Remove(this);
            }
        }

        public void RemoveAll()
        {
            this.RemoveAllChildren();
            if (this.HasAttributes)
            {
                this._attributes.Clear();
            }
            if (((this._endnode != null) && (this._endnode != this)) && (this._endnode._attributes != null))
            {
                this._endnode._attributes.Clear();
            }
            this._outerchanged = true;
            this._innerchanged = true;
        }

        public void RemoveAllChildren()
        {
            if (this.HasChildNodes)
            {
                if (this._ownerdocument.OptionUseIdAttribute)
                {
                    foreach (HtmlNode node in (IEnumerable<HtmlNode>)this._childnodes)
                    {
                        this._ownerdocument.SetIdForNode(null, node.GetId());
                    }
                }
                this._childnodes.Clear();
                this._outerchanged = true;
                this._innerchanged = true;
            }
        }

        public HtmlNode RemoveChild(HtmlNode oldChild)
        {
            if (oldChild == null)
            {
                throw new ArgumentNullException("oldChild");
            }
            int index = -1;
            if (this._childnodes != null)
            {
                index = this._childnodes[oldChild];
            }
            if (index == -1)
            {
                throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
            }
            if (this._childnodes != null)
            {
                this._childnodes.Remove(index);
            }
            this._ownerdocument.SetIdForNode(null, oldChild.GetId());
            this._outerchanged = true;
            this._innerchanged = true;
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
                    this.InsertAfter(node2, previousSibling);
                }
            }
            this.RemoveChild(oldChild);
            this._outerchanged = true;
            this._innerchanged = true;
            return oldChild;
        }

        public HtmlNode ReplaceChild(HtmlNode newChild, HtmlNode oldChild)
        {
            if (newChild == null)
            {
                return this.RemoveChild(oldChild);
            }
            if (oldChild == null)
            {
                return this.AppendChild(newChild);
            }
            int index = -1;
            if (this._childnodes != null)
            {
                index = this._childnodes[oldChild];
            }
            if (index == -1)
            {
                throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
            }
            if (this._childnodes != null)
            {
                this._childnodes.Replace(index, newChild);
            }
            this._ownerdocument.SetIdForNode(null, oldChild.GetId());
            this._ownerdocument.SetIdForNode(newChild, newChild.GetId());
            this._outerchanged = true;
            this._innerchanged = true;
            return newChild;
        }

        public HtmlNodeCollection SelectNodes(string xpath)
        {
            HtmlNodeCollection nodes = new HtmlNodeCollection(null);
            XPathNodeIterator iterator = new HtmlNodeNavigator(this.OwnerDocument, this).Select(xpath);
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
            XPathNodeIterator iterator = new HtmlNodeNavigator(this.OwnerDocument, this).Select(xpath);
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
            HtmlAttribute attribute = this.Attributes[name];
            if (attribute == null)
            {
                return this.Attributes.Append(this._ownerdocument.CreateAttribute(name, value));
            }
            attribute.Value = value;
            return attribute;
        }

        internal void SetId(string id)
        {
            HtmlAttribute attribute = this.Attributes["id"] ?? this._ownerdocument.CreateAttribute("id");
            attribute.Value = id;
            this._ownerdocument.SetIdForNode(this, attribute.Value);
            this._outerchanged = true;
        }

        internal void WriteAttribute(TextWriter outText, HtmlAttribute att)
        {
            string originalName;
            string str2 = (att.QuoteType == AttributeValueQuote.DoubleQuote) ? "\"" : "'";
            if (this._ownerdocument.OptionOutputAsXml)
            {
                originalName = this._ownerdocument.OptionOutputUpperCase ? att.XmlName.ToUpper() : att.XmlName;
                if (this._ownerdocument.OptionOutputOriginalCase)
                {
                    originalName = att.OriginalName;
                }
                outText.Write(" " + originalName + "=" + str2 + HtmlDocument.HtmlEncode(att.XmlValue) + str2);
            }
            else
            {
                originalName = this._ownerdocument.OptionOutputUpperCase ? att.Name.ToUpper() : att.Name;
                if ((((att.Name.Length >= 4) && (att.Name[0] == '<')) && ((att.Name[1] == '%') && (att.Name[att.Name.Length - 1] == '>'))) && (att.Name[att.Name.Length - 2] == '%'))
                {
                    outText.Write(" " + originalName);
                }
                else if (this._ownerdocument.OptionOutputOptimizeAttributeValues)
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
            if (this._ownerdocument.OptionOutputAsXml)
            {
                if (this._attributes != null)
                {
                    foreach (HtmlAttribute attribute in this._attributes.Hashitems.Values)
                    {
                        this.WriteAttribute(outText, attribute);
                    }
                }
            }
            else if (!closing)
            {
                if (this._attributes != null)
                {
                    foreach (HtmlAttribute attribute2 in (IEnumerable<HtmlAttribute>)this._attributes)
                    {
                        this.WriteAttribute(outText, attribute2);
                    }
                }
                if (this._ownerdocument.OptionAddDebuggingAttributes)
                {
                    this.WriteAttribute(outText, this._ownerdocument.CreateAttribute("_closed", this.Closed.ToString()));
                    this.WriteAttribute(outText, this._ownerdocument.CreateAttribute("_children", this.ChildNodes.Count.ToString()));
                    int num = 0;
                    foreach (HtmlNode node in (IEnumerable<HtmlNode>)this.ChildNodes)
                    {
                        this.WriteAttribute(outText, this._ownerdocument.CreateAttribute("_child_" + num, node.Name));
                        num++;
                    }
                }
            }
            else if (((this._endnode != null) && (this._endnode._attributes != null)) && (this._endnode != this))
            {
                foreach (HtmlAttribute attribute3 in (IEnumerable<HtmlAttribute>)this._endnode._attributes)
                {
                    this.WriteAttribute(outText, attribute3);
                }
                if (this._ownerdocument.OptionAddDebuggingAttributes)
                {
                    this.WriteAttribute(outText, this._ownerdocument.CreateAttribute("_closed", this.Closed.ToString()));
                    this.WriteAttribute(outText, this._ownerdocument.CreateAttribute("_children", this.ChildNodes.Count.ToString()));
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
            this.WriteContentTo(outText);
            outText.Flush();
            return outText.ToString();
        }

        public void WriteContentTo(TextWriter outText)
        {
            if (this._childnodes != null)
            {
                foreach (HtmlNode node in (IEnumerable<HtmlNode>)this._childnodes)
                {
                    node.WriteTo(outText);
                }
            }
        }

        public string WriteTo()
        {
            using (StringWriter writer = new StringWriter())
            {
                this.WriteTo(writer);
                writer.Flush();
                return writer.ToString();
            }
        }

        public void WriteTo(TextWriter outText)
        {
            string comment;
            switch (this._nodetype)
            {
                case HtmlNodeType.Document:
                    {
                        if (!this._ownerdocument.OptionOutputAsXml)
                        {
                            break;
                        }
                        outText.Write("<?xml version=\"1.0\" encoding=\"" + this._ownerdocument.GetOutEncoding().BodyName + "\"?>");
                        if (!this._ownerdocument.DocumentNode.HasChildNodes)
                        {
                            break;
                        }
                        int count = this._ownerdocument.DocumentNode._childnodes.Count;
                        if (count <= 0)
                        {
                            break;
                        }
                        if (this._ownerdocument.GetXmlDeclaration() != null)
                        {
                            count--;
                        }
                        if (count <= 1)
                        {
                            break;
                        }
                        if (this._ownerdocument.OptionOutputUpperCase)
                        {
                            outText.Write("<SPAN>");
                            this.WriteContentTo(outText);
                            outText.Write("</SPAN>");
                            return;
                        }
                        outText.Write("<span>");
                        this.WriteContentTo(outText);
                        outText.Write("</span>");
                        return;
                    }
                case HtmlNodeType.Element:
                    {
                        string name = this._ownerdocument.OptionOutputUpperCase ? this.Name.ToUpper() : this.Name;
                        if (this._ownerdocument.OptionOutputOriginalCase)
                        {
                            name = this.OriginalName;
                        }
                        if (this._ownerdocument.OptionOutputAsXml)
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
                        this.WriteAttributes(outText, false);
                        if (!this.HasChildNodes)
                        {
                            if (IsEmptyElement(this.Name))
                            {
                                if (this._ownerdocument.OptionWriteEmptyNodes || this._ownerdocument.OptionOutputAsXml)
                                {
                                    outText.Write(" />");
                                    return;
                                }
                                if ((this.Name.Length > 0) && (this.Name[0] == '?'))
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
                        if (this._ownerdocument.OptionOutputAsXml && IsCDataElement(this.Name))
                        {
                            flag = true;
                            outText.Write("\r\n//<![CDATA[\r\n");
                        }
                        if (flag)
                        {
                            if (this.HasChildNodes)
                            {
                                this.ChildNodes[0].WriteTo(outText);
                            }
                            outText.Write("\r\n//]]>//\r\n");
                        }
                        else
                        {
                            this.WriteContentTo(outText);
                        }
                        outText.Write("</" + name);
                        if (!this._ownerdocument.OptionOutputAsXml)
                        {
                            this.WriteAttributes(outText, true);
                        }
                        outText.Write(">");
                        return;
                    }
                case HtmlNodeType.Comment:
                    comment = ((HtmlCommentNode)this).Comment;
                    if (!this._ownerdocument.OptionOutputAsXml)
                    {
                        outText.Write(comment);
                        return;
                    }
                    outText.Write("<!--" + GetXmlComment((HtmlCommentNode)this) + " -->");
                    return;

                case HtmlNodeType.Text:
                    comment = ((HtmlTextNode)this).Text;
                    outText.Write(this._ownerdocument.OptionOutputAsXml ? HtmlDocument.HtmlEncode(comment) : comment);
                    return;

                default:
                    return;
            }
            this.WriteContentTo(outText);
        }

        public void WriteTo(XmlWriter writer)
        {
            switch (this._nodetype)
            {
                case HtmlNodeType.Document:
                    writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"" + this._ownerdocument.GetOutEncoding().BodyName + "\"");
                    if (this.HasChildNodes)
                    {
                        foreach (HtmlNode node in (IEnumerable<HtmlNode>)this.ChildNodes)
                        {
                            node.WriteTo(writer);
                        }
                    }
                    break;

                case HtmlNodeType.Element:
                    {
                        string localName = this._ownerdocument.OptionOutputUpperCase ? this.Name.ToUpper() : this.Name;
                        if (this._ownerdocument.OptionOutputOriginalCase)
                        {
                            localName = this.OriginalName;
                        }
                        writer.WriteStartElement(localName);
                        WriteAttributes(writer, this);
                        if (this.HasChildNodes)
                        {
                            foreach (HtmlNode node2 in (IEnumerable<HtmlNode>)this.ChildNodes)
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
                if (!this.HasAttributes)
                {
                    this._attributes = new HtmlAttributeCollection(this);
                }
                return this._attributes;
            }
            internal set
            {
                this._attributes = value;
            }
        }

        public bool Closed
        {
            get
            {
                return (this._endnode != null);
            }
        }

        public HtmlAttributeCollection ClosingAttributes
        {
            get
            {
                if (this.HasClosingAttributes)
                {
                    return this._endnode.Attributes;
                }
                return new HtmlAttributeCollection(this);
            }
        }

        public HtmlNodeCollection ChildNodes
        {
            get
            {
                return (this._childnodes ?? (this._childnodes = new HtmlNodeCollection(this)));
            }
            internal set
            {
                this._childnodes = value;
            }
        }

        internal HtmlNode EndNode
        {
            get
            {
                return this._endnode;
            }
        }

        public HtmlNode FirstChild
        {
            get
            {
                if (this.HasChildNodes)
                {
                    return this._childnodes[0];
                }
                return null;
            }
        }

        public bool HasAttributes
        {
            get
            {
                if (this._attributes == null)
                {
                    return false;
                }
                if (this._attributes.Count <= 0)
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
                if ((this._endnode == null) || (this._endnode == this))
                {
                    return false;
                }
                if (this._endnode._attributes == null)
                {
                    return false;
                }
                if (this._endnode._attributes.Count <= 0)
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
                if (this._childnodes == null)
                {
                    return false;
                }
                if (this._childnodes.Count <= 0)
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
                if (this._ownerdocument.Nodesid == null)
                {
                    throw new Exception(HtmlDocument.HtmlExceptionUseIdAttributeFalse);
                }
                return this.GetId();
            }
            set
            {
                if (this._ownerdocument.Nodesid == null)
                {
                    throw new Exception(HtmlDocument.HtmlExceptionUseIdAttributeFalse);
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.SetId(value);
            }
        }

        public virtual string InnerHtml
        {
            get
            {
                if (this._innerchanged)
                {
                    this._innerhtml = this.WriteContentTo();
                    this._innerchanged = false;
                    return this._innerhtml;
                }
                if (this._innerhtml != null)
                {
                    return this._innerhtml;
                }
                if (this._innerstartindex < 0)
                {
                    return string.Empty;
                }
                return this._ownerdocument.Text.Substring(this._innerstartindex, this._innerlength);
            }
            set
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(value);
                this.RemoveAllChildren();
                this.AppendChildren(document.DocumentNode.ChildNodes);
            }
        }

        public virtual string InnerText
        {
            get
            {
                if (this._nodetype == HtmlNodeType.Text)
                {
                    return ((HtmlTextNode)this).Text;
                }
                if (this._nodetype == HtmlNodeType.Comment)
                {
                    return ((HtmlCommentNode)this).Comment;
                }
                if (!this.HasChildNodes)
                {
                    return string.Empty;
                }
                string str = null;
                foreach (HtmlNode node in (IEnumerable<HtmlNode>)this.ChildNodes)
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
                if (this.HasChildNodes)
                {
                    return this._childnodes[this._childnodes.Count - 1];
                }
                return null;
            }
        }

        public int Line
        {
            get
            {
                return this._line;
            }
            internal set
            {
                this._line = value;
            }
        }

        public int LinePosition
        {
            get
            {
                return this._lineposition;
            }
            internal set
            {
                this._lineposition = value;
            }
        }

        public string Name
        {
            get
            {
                if (this._optimizedName == null)
                {
                    if (this._name == null)
                    {
                        this.Name = this._ownerdocument.Text.Substring(this._namestartindex, this._namelength);
                    }
                    if (this._name == null)
                    {
                        this._optimizedName = string.Empty;
                    }
                    else
                    {
                        this._optimizedName = this._name.ToLower();
                    }
                }
                return this._optimizedName;
            }
            set
            {
                this._name = value;
                this._optimizedName = null;
            }
        }

        public HtmlNode NextSibling
        {
            get
            {
                return this._nextnode;
            }
            internal set
            {
                this._nextnode = value;
            }
        }

        public HtmlNodeType NodeType
        {
            get
            {
                return this._nodetype;
            }
            internal set
            {
                this._nodetype = value;
            }
        }

        public string OriginalName
        {
            get
            {
                return this._name;
            }
        }

        public virtual string OuterHtml
        {
            get
            {
                if (this._outerchanged)
                {
                    this._outerhtml = this.WriteTo();
                    this._outerchanged = false;
                    return this._outerhtml;
                }
                if (this._outerhtml != null)
                {
                    return this._outerhtml;
                }
                if (this._outerstartindex < 0)
                {
                    return string.Empty;
                }
                return this._ownerdocument.Text.Substring(this._outerstartindex, this._outerlength);
            }
        }

        public HtmlDocument OwnerDocument
        {
            get
            {
                return this._ownerdocument;
            }
            internal set
            {
                this._ownerdocument = value;
            }
        }

        public HtmlNode ParentNode
        {
            get
            {
                return this._parentnode;
            }
            internal set
            {
                this._parentnode = value;
            }
        }

        public HtmlNode PreviousSibling
        {
            get
            {
                return this._prevnode;
            }
            internal set
            {
                this._prevnode = value;
            }
        }

        public int StreamPosition
        {
            get
            {
                return this._streamposition;
            }
        }

        public string XPath
        {
            get
            {
                string str = ((this.ParentNode == null) || (this.ParentNode.NodeType == HtmlNodeType.Document)) ? "/" : (this.ParentNode.XPath + "/");
                return (str + this.GetRelativeXpath());
            }
        }
    }
}