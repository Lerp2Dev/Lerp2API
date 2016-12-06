namespace HtmlAgilityPack
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;

    public class HtmlNodeNavigator : XPathNavigator
    {
        private int _attindex;
        private HtmlNode _currentnode;
        private readonly HtmlDocument _doc;
        private readonly HtmlNameTable _nametable;
        internal bool Trace;

        internal HtmlNodeNavigator()
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            Reset();
        }

        private HtmlNodeNavigator(HtmlNodeNavigator nav)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            if (nav == null)
            {
                throw new ArgumentNullException("nav");
            }
            _doc = nav._doc;
            _currentnode = nav._currentnode;
            _attindex = nav._attindex;
            _nametable = nav._nametable;
        }

        public HtmlNodeNavigator(Stream stream)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(stream);
            Reset();
        }

        public HtmlNodeNavigator(TextReader reader)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(reader);
            Reset();
        }

        public HtmlNodeNavigator(string path)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(path);
            Reset();
        }

        internal HtmlNodeNavigator(HtmlDocument doc, HtmlNode currentNode)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            if (currentNode == null)
            {
                throw new ArgumentNullException("currentNode");
            }
            if (currentNode.OwnerDocument != doc)
            {
                throw new ArgumentException(HtmlDocument.HtmlExceptionRefNotChild);
            }
            _doc = doc;
            Reset();
            _currentnode = currentNode;
        }

        public HtmlNodeNavigator(Stream stream, bool detectEncodingFromByteOrderMarks)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(stream, detectEncodingFromByteOrderMarks);
            Reset();
        }

        public HtmlNodeNavigator(Stream stream, Encoding encoding)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(stream, encoding);
            Reset();
        }

        public HtmlNodeNavigator(string path, bool detectEncodingFromByteOrderMarks)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(path, detectEncodingFromByteOrderMarks);
            Reset();
        }

        public HtmlNodeNavigator(string path, Encoding encoding)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(path, encoding);
            Reset();
        }

        public HtmlNodeNavigator(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(stream, encoding, detectEncodingFromByteOrderMarks);
            Reset();
        }

        public HtmlNodeNavigator(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(path, encoding, detectEncodingFromByteOrderMarks);
            Reset();
        }

        public HtmlNodeNavigator(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(stream, encoding, detectEncodingFromByteOrderMarks, buffersize);
            Reset();
        }

        public HtmlNodeNavigator(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(path, encoding, detectEncodingFromByteOrderMarks, buffersize);
            Reset();
        }

        public override XPathNavigator Clone()
        {
            return new HtmlNodeNavigator(this);
        }

        public override string GetAttribute(string localName, string namespaceURI)
        {
            HtmlAttribute attribute = _currentnode.Attributes[localName];
            if (attribute == null)
            {
                return null;
            }
            return attribute.Value;
        }

        public override string GetNamespace(string name)
        {
            return string.Empty;
        }

        [Conditional("TRACE")]
        internal void InternalTrace(object traceValue)
        {
            string outerHtml;
            if (!Trace)
            {
                return;
            }
            StackFrame frame = new StackFrame(1, true);
            string name = frame.GetMethod().Name;
            string str2 = (_currentnode == null) ? "(null)" : _currentnode.Name;
            if (_currentnode == null)
            {
                outerHtml = "(null)";
            }
            else
            {
                switch (_currentnode.NodeType)
                {
                    case HtmlNodeType.Document:
                        outerHtml = "";
                        goto Label_00AE;

                    case HtmlNodeType.Comment:
                        outerHtml = ((HtmlCommentNode)_currentnode).Comment;
                        goto Label_00AE;

                    case HtmlNodeType.Text:
                        outerHtml = ((HtmlTextNode)_currentnode).Text;
                        goto Label_00AE;
                }
                outerHtml = _currentnode.CloneNode(false).OuterHtml;
            }
            Label_00AE:;
            HtmlAgilityPack.Trace.WriteLine(string.Format("oid={0},n={1},a={2},v={3},{4}", new object[] { GetHashCode(), str2, _attindex, outerHtml, traceValue }), "N!" + name);
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            HtmlNodeNavigator navigator = other as HtmlNodeNavigator;
            if (navigator == null)
            {
                return false;
            }
            return (navigator._currentnode == _currentnode);
        }

        public override bool MoveTo(XPathNavigator other)
        {
            HtmlNodeNavigator navigator = other as HtmlNodeNavigator;
            if ((navigator != null) && (navigator._doc == _doc))
            {
                _currentnode = navigator._currentnode;
                _attindex = navigator._attindex;
                return true;
            }
            return false;
        }

        public override bool MoveToAttribute(string localName, string namespaceURI)
        {
            int attributeIndex = _currentnode.Attributes.GetAttributeIndex(localName);
            if (attributeIndex == -1)
            {
                return false;
            }
            _attindex = attributeIndex;
            return true;
        }

        public override bool MoveToFirst()
        {
            if (_currentnode.ParentNode == null)
            {
                return false;
            }
            if (_currentnode.ParentNode.FirstChild == null)
            {
                return false;
            }
            _currentnode = _currentnode.ParentNode.FirstChild;
            return true;
        }

        public override bool MoveToFirstAttribute()
        {
            if (!HasAttributes)
            {
                return false;
            }
            _attindex = 0;
            return true;
        }

        public override bool MoveToFirstChild()
        {
            if (!_currentnode.HasChildNodes)
            {
                return false;
            }
            _currentnode = _currentnode.ChildNodes[0];
            return true;
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope scope)
        {
            return false;
        }

        public override bool MoveToId(string id)
        {
            HtmlNode elementbyId = _doc.GetElementbyId(id);
            if (elementbyId == null)
            {
                return false;
            }
            _currentnode = elementbyId;
            return true;
        }

        public override bool MoveToNamespace(string name)
        {
            return false;
        }

        public override bool MoveToNext()
        {
            if (_currentnode.NextSibling == null)
            {
                return false;
            }
            _currentnode = _currentnode.NextSibling;
            return true;
        }

        public override bool MoveToNextAttribute()
        {
            if (_attindex >= (_currentnode.Attributes.Count - 1))
            {
                return false;
            }
            _attindex++;
            return true;
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope scope)
        {
            return false;
        }

        public override bool MoveToParent()
        {
            if (_currentnode.ParentNode == null)
            {
                return false;
            }
            _currentnode = _currentnode.ParentNode;
            return true;
        }

        public override bool MoveToPrevious()
        {
            if (_currentnode.PreviousSibling == null)
            {
                return false;
            }
            _currentnode = _currentnode.PreviousSibling;
            return true;
        }

        public override void MoveToRoot()
        {
            _currentnode = _doc.DocumentNode;
        }

        private void Reset()
        {
            _currentnode = _doc.DocumentNode;
            _attindex = -1;
        }

        public override string BaseURI
        {
            get
            {
                return _nametable.GetOrAdd(string.Empty);
            }
        }

        public HtmlDocument CurrentDocument
        {
            get
            {
                return _doc;
            }
        }

        public HtmlNode CurrentNode
        {
            get
            {
                return _currentnode;
            }
        }

        public override bool HasAttributes
        {
            get
            {
                return (_currentnode.Attributes.Count > 0);
            }
        }

        public override bool HasChildren
        {
            get
            {
                return (_currentnode.ChildNodes.Count > 0);
            }
        }

        public override bool IsEmptyElement
        {
            get
            {
                return !HasChildren;
            }
        }

        public override string LocalName
        {
            get
            {
                if (_attindex != -1)
                {
                    return _nametable.GetOrAdd(_currentnode.Attributes[_attindex].Name);
                }
                return _nametable.GetOrAdd(_currentnode.Name);
            }
        }

        public override string Name
        {
            get
            {
                return _nametable.GetOrAdd(_currentnode.Name);
            }
        }

        public override string NamespaceURI
        {
            get
            {
                return _nametable.GetOrAdd(string.Empty);
            }
        }

        public override XmlNameTable NameTable
        {
            get
            {
                return _nametable;
            }
        }

        public override XPathNodeType NodeType
        {
            get
            {
                switch (_currentnode.NodeType)
                {
                    case HtmlNodeType.Document:
                        return XPathNodeType.Root;

                    case HtmlNodeType.Element:
                        if (_attindex == -1)
                        {
                            return XPathNodeType.Element;
                        }
                        return XPathNodeType.Attribute;

                    case HtmlNodeType.Comment:
                        return XPathNodeType.Comment;

                    case HtmlNodeType.Text:
                        return XPathNodeType.Text;
                }
                throw new NotImplementedException("Internal error: Unhandled HtmlNodeType: " + _currentnode.NodeType);
            }
        }

        public override string Prefix
        {
            get
            {
                return _nametable.GetOrAdd(string.Empty);
            }
        }

        public override string Value
        {
            get
            {
                switch (_currentnode.NodeType)
                {
                    case HtmlNodeType.Document:
                        return "";

                    case HtmlNodeType.Element:
                        if (_attindex == -1)
                        {
                            return _currentnode.InnerText;
                        }
                        return _currentnode.Attributes[_attindex].Value;

                    case HtmlNodeType.Comment:
                        return ((HtmlCommentNode)_currentnode).Comment;

                    case HtmlNodeType.Text:
                        return ((HtmlTextNode)_currentnode).Text;
                }
                throw new NotImplementedException("Internal error: Unhandled HtmlNodeType: " + _currentnode.NodeType);
            }
        }

        public override string XmlLang
        {
            get
            {
                return _nametable.GetOrAdd(string.Empty);
            }
        }
    }
}