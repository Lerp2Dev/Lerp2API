namespace HtmlAgilityPack
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;

    /// <summary>
    /// Class HtmlNodeNavigator.
    /// </summary>
    /// <seealso cref="System.Xml.XPath.XPathNavigator" />
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

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeNavigator"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public HtmlNodeNavigator(Stream stream)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(stream);
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeNavigator"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public HtmlNodeNavigator(TextReader reader)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(reader);
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeNavigator"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeNavigator"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public HtmlNodeNavigator(Stream stream, bool detectEncodingFromByteOrderMarks)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(stream, detectEncodingFromByteOrderMarks);
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeNavigator"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        public HtmlNodeNavigator(Stream stream, Encoding encoding)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(stream, encoding);
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeNavigator"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public HtmlNodeNavigator(string path, bool detectEncodingFromByteOrderMarks)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(path, detectEncodingFromByteOrderMarks);
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeNavigator"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        public HtmlNodeNavigator(string path, Encoding encoding)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(path, encoding);
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeNavigator"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public HtmlNodeNavigator(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(stream, encoding, detectEncodingFromByteOrderMarks);
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeNavigator"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public HtmlNodeNavigator(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(path, encoding, detectEncodingFromByteOrderMarks);
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeNavigator"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        /// <param name="buffersize">The buffersize.</param>
        public HtmlNodeNavigator(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(stream, encoding, detectEncodingFromByteOrderMarks, buffersize);
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNodeNavigator"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        /// <param name="buffersize">The buffersize.</param>
        public HtmlNodeNavigator(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            _doc = new HtmlDocument();
            _nametable = new HtmlNameTable();
            _doc.Load(path, encoding, detectEncodingFromByteOrderMarks, buffersize);
            Reset();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>XPathNavigator.</returns>
        public override XPathNavigator Clone()
        {
            return new HtmlNodeNavigator(this);
        }

        /// <summary>
        /// Gets the attribute.
        /// </summary>
        /// <param name="localName">Name of the local.</param>
        /// <param name="namespaceURI">The namespace URI.</param>
        /// <returns>System.String.</returns>
        public override string GetAttribute(string localName, string namespaceURI)
        {
            HtmlAttribute attribute = _currentnode.Attributes[localName];
            if (attribute == null)
            {
                return null;
            }
            return attribute.Value;
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
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

        /// <summary>
        /// Determines whether [is same position] [the specified other].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if [is same position] [the specified other]; otherwise, <c>false</c>.</returns>
        public override bool IsSamePosition(XPathNavigator other)
        {
            HtmlNodeNavigator navigator = other as HtmlNodeNavigator;
            if (navigator == null)
            {
                return false;
            }
            return (navigator._currentnode == _currentnode);
        }

        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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

        /// <summary>
        /// Moves to attribute.
        /// </summary>
        /// <param name="localName">Name of the local.</param>
        /// <param name="namespaceURI">The namespace URI.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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

        /// <summary>
        /// Moves to first.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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

        /// <summary>
        /// Moves to first attribute.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool MoveToFirstAttribute()
        {
            if (!HasAttributes)
            {
                return false;
            }
            _attindex = 0;
            return true;
        }

        /// <summary>
        /// Moves to first child.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool MoveToFirstChild()
        {
            if (!_currentnode.HasChildNodes)
            {
                return false;
            }
            _currentnode = _currentnode.ChildNodes[0];
            return true;
        }

        /// <summary>
        /// Moves to first namespace.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool MoveToFirstNamespace(XPathNamespaceScope scope)
        {
            return false;
        }

        /// <summary>
        /// Moves to identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
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

        /// <summary>
        /// Moves to namespace.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool MoveToNamespace(string name)
        {
            return false;
        }

        /// <summary>
        /// Moves to next.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool MoveToNext()
        {
            if (_currentnode.NextSibling == null)
            {
                return false;
            }
            _currentnode = _currentnode.NextSibling;
            return true;
        }

        /// <summary>
        /// Moves to next attribute.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool MoveToNextAttribute()
        {
            if (_attindex >= (_currentnode.Attributes.Count - 1))
            {
                return false;
            }
            _attindex++;
            return true;
        }

        /// <summary>
        /// Moves to next namespace.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool MoveToNextNamespace(XPathNamespaceScope scope)
        {
            return false;
        }

        /// <summary>
        /// Moves to parent.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool MoveToParent()
        {
            if (_currentnode.ParentNode == null)
            {
                return false;
            }
            _currentnode = _currentnode.ParentNode;
            return true;
        }

        /// <summary>
        /// Moves to previous.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool MoveToPrevious()
        {
            if (_currentnode.PreviousSibling == null)
            {
                return false;
            }
            _currentnode = _currentnode.PreviousSibling;
            return true;
        }

        /// <summary>
        /// Moves to root.
        /// </summary>
        public override void MoveToRoot()
        {
            _currentnode = _doc.DocumentNode;
        }

        private void Reset()
        {
            _currentnode = _doc.DocumentNode;
            _attindex = -1;
        }

        /// <summary>
        /// Gets the base URI.
        /// </summary>
        /// <value>The base URI.</value>
        public override string BaseURI
        {
            get
            {
                return _nametable.GetOrAdd(string.Empty);
            }
        }

        /// <summary>
        /// Gets the current document.
        /// </summary>
        /// <value>The current document.</value>
        public HtmlDocument CurrentDocument
        {
            get
            {
                return _doc;
            }
        }

        /// <summary>
        /// Gets the current node.
        /// </summary>
        /// <value>The current node.</value>
        public HtmlNode CurrentNode
        {
            get
            {
                return _currentnode;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has attributes.
        /// </summary>
        /// <value><c>true</c> if this instance has attributes; otherwise, <c>false</c>.</value>
        public override bool HasAttributes
        {
            get
            {
                return (_currentnode.Attributes.Count > 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has children.
        /// </summary>
        /// <value><c>true</c> if this instance has children; otherwise, <c>false</c>.</value>
        public override bool HasChildren
        {
            get
            {
                return (_currentnode.ChildNodes.Count > 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty element.
        /// </summary>
        /// <value><c>true</c> if this instance is empty element; otherwise, <c>false</c>.</value>
        public override bool IsEmptyElement
        {
            get
            {
                return !HasChildren;
            }
        }

        /// <summary>
        /// Gets the name of the local.
        /// </summary>
        /// <value>The name of the local.</value>
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

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public override string Name
        {
            get
            {
                return _nametable.GetOrAdd(_currentnode.Name);
            }
        }

        /// <summary>
        /// Gets the namespace URI.
        /// </summary>
        /// <value>The namespace URI.</value>
        public override string NamespaceURI
        {
            get
            {
                return _nametable.GetOrAdd(string.Empty);
            }
        }

        /// <summary>
        /// Gets the name table.
        /// </summary>
        /// <value>The name table.</value>
        public override XmlNameTable NameTable
        {
            get
            {
                return _nametable;
            }
        }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>The type of the node.</value>
        /// <exception cref="NotImplementedException">Internal error: Unhandled HtmlNodeType: " + _currentnode.NodeType</exception>
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

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        /// <value>The prefix.</value>
        public override string Prefix
        {
            get
            {
                return _nametable.GetOrAdd(string.Empty);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        /// <exception cref="NotImplementedException">Internal error: Unhandled HtmlNodeType: " + _currentnode.NodeType</exception>
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

        /// <summary>
        /// Gets the XML language.
        /// </summary>
        /// <value>The XML language.</value>
        public override string XmlLang
        {
            get
            {
                return _nametable.GetOrAdd(string.Empty);
            }
        }
    }
}