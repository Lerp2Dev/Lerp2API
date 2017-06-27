namespace HtmlAgilityPack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.XPath;

    /// <summary>
    /// Class HtmlDocument.
    /// </summary>
    /// <seealso cref="System.Xml.XPath.IXPathNavigable" />
    public class HtmlDocument : IXPathNavigable
    {
        private int _c;
        private Crc32 _crc32;
        private HtmlAttribute _currentattribute;
        private HtmlNode _currentnode;
        private Encoding _declaredencoding;
        private HtmlNode _documentnode;
        private bool _fullcomment;
        private int _index;
        private HtmlNode _lastparentnode;
        private int _line;
        private int _lineposition;
        private int _maxlineposition;
        private ParseState _oldstate;
        private bool _onlyDetectEncoding;
        private List<HtmlParseError> _parseerrors = new List<HtmlParseError>();
        private string _remainder;
        private int _remainderOffset;
        private ParseState _state;
        private Encoding _streamencoding;
        internal static readonly string HtmlExceptionRefNotChild = "Reference node must be a child of this node";
        internal static readonly string HtmlExceptionUseIdAttributeFalse = "You need to set UseIdAttribute property to true to enable this feature";
        internal Dictionary<string, HtmlNode> Lastnodes = new Dictionary<string, HtmlNode>();
        internal Dictionary<string, HtmlNode> Nodesid;
        internal Dictionary<int, HtmlNode> Openednodes;

        /// <summary>
        /// The option add debugging attributes
        /// </summary>
        public bool OptionAddDebuggingAttributes;

        /// <summary>
        /// The option automatic close on end
        /// </summary>
        public bool OptionAutoCloseOnEnd;

        /// <summary>
        /// The option compute checksum
        /// </summary>
        public bool OptionComputeChecksum;

        /// <summary>
        /// The option check syntax
        /// </summary>
        public bool OptionCheckSyntax = true;

        /// <summary>
        /// The option default stream encoding
        /// </summary>
        public Encoding OptionDefaultStreamEncoding;

        /// <summary>
        /// The option extract error source text
        /// </summary>
        public bool OptionExtractErrorSourceText;

        /// <summary>
        /// The option extract error source text maximum length
        /// </summary>
        public int OptionExtractErrorSourceTextMaxLength = 100;

        /// <summary>
        /// The option fix nested tags
        /// </summary>
        public bool OptionFixNestedTags;

        /// <summary>
        /// The option output as XML
        /// </summary>
        public bool OptionOutputAsXml;

        /// <summary>
        /// The option output optimize attribute values
        /// </summary>
        public bool OptionOutputOptimizeAttributeValues;

        /// <summary>
        /// The option output original case
        /// </summary>
        public bool OptionOutputOriginalCase;

        /// <summary>
        /// The option output upper case
        /// </summary>
        public bool OptionOutputUpperCase;

        /// <summary>
        /// The option read encoding
        /// </summary>
        public bool OptionReadEncoding = true;

        /// <summary>
        /// The option stopper node name
        /// </summary>
        public string OptionStopperNodeName;

        /// <summary>
        /// The option use identifier attribute
        /// </summary>
        public bool OptionUseIdAttribute = true;

        /// <summary>
        /// The option write empty nodes
        /// </summary>
        public bool OptionWriteEmptyNodes;

        internal string Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlDocument"/> class.
        /// </summary>
        public HtmlDocument()
        {
            _documentnode = CreateNode(HtmlNodeType.Document, 0);
            OptionDefaultStreamEncoding = Encoding.Default;
        }

        private void AddError(HtmlParseErrorCode code, int line, int linePosition, int streamPosition, string sourceText, string reason)
        {
            HtmlParseError item = new HtmlParseError(code, line, linePosition, streamPosition, sourceText, reason);
            _parseerrors.Add(item);
        }

        private void CloseCurrentNode()
        {
            if (!_currentnode.Closed)
            {
                bool flag = false;
                HtmlNode dictionaryValueOrNull = Utilities.GetDictionaryValueOrNull<string, HtmlNode>(Lastnodes, _currentnode.Name);
                if (dictionaryValueOrNull != null)
                {
                    if (OptionFixNestedTags && FindResetterNodes(dictionaryValueOrNull, GetResetters(_currentnode.Name)))
                    {
                        AddError(HtmlParseErrorCode.EndTagInvalidHere, _currentnode._line, _currentnode._lineposition, _currentnode._streamposition, _currentnode.OuterHtml, "End tag </" + _currentnode.Name + "> invalid here");
                        flag = true;
                    }
                    if (!flag)
                    {
                        Lastnodes[_currentnode.Name] = dictionaryValueOrNull._prevwithsamename;
                        dictionaryValueOrNull.CloseNode(_currentnode);
                    }
                }
                else if (!HtmlNode.IsClosedElement(_currentnode.Name))
                {
                    if (HtmlNode.CanOverlapElement(_currentnode.Name))
                    {
                        HtmlNode newChild = CreateNode(HtmlNodeType.Text, _currentnode._outerstartindex);
                        newChild._outerlength = _currentnode._outerlength;
                        ((HtmlTextNode)newChild).Text = ((HtmlTextNode)newChild).Text.ToLower();
                        if (_lastparentnode != null)
                        {
                            _lastparentnode.AppendChild(newChild);
                        }
                    }
                    else if (HtmlNode.IsEmptyElement(_currentnode.Name))
                    {
                        AddError(HtmlParseErrorCode.EndTagNotRequired, _currentnode._line, _currentnode._lineposition, _currentnode._streamposition, _currentnode.OuterHtml, "End tag </" + _currentnode.Name + "> is not required");
                    }
                    else
                    {
                        AddError(HtmlParseErrorCode.TagNotOpened, _currentnode._line, _currentnode._lineposition, _currentnode._streamposition, _currentnode.OuterHtml, "Start tag <" + _currentnode.Name + "> was not found");
                        flag = true;
                    }
                }
                else
                {
                    _currentnode.CloseNode(_currentnode);
                    if (_lastparentnode != null)
                    {
                        HtmlNode node2 = null;
                        Stack<HtmlNode> stack = new Stack<HtmlNode>();
                        for (HtmlNode node3 = _lastparentnode.LastChild; node3 != null; node3 = node3.PreviousSibling)
                        {
                            if ((node3.Name == _currentnode.Name) && !node3.HasChildNodes)
                            {
                                node2 = node3;
                                break;
                            }
                            stack.Push(node3);
                        }
                        if (node2 != null)
                        {
                            while (stack.Count != 0)
                            {
                                HtmlNode oldChild = stack.Pop();
                                _lastparentnode.RemoveChild(oldChild);
                                node2.AppendChild(oldChild);
                            }
                        }
                        else
                        {
                            _lastparentnode.AppendChild(_currentnode);
                        }
                    }
                }
                if ((!flag && (_lastparentnode != null)) && (!HtmlNode.IsClosedElement(_currentnode.Name) || _currentnode._starttag))
                {
                    UpdateLastParentNode();
                }
            }
        }

        internal HtmlAttribute CreateAttribute()
        {
            return new HtmlAttribute(this);
        }

        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>HtmlAttribute.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public HtmlAttribute CreateAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            HtmlAttribute attribute = CreateAttribute();
            attribute.Name = name;
            return attribute;
        }

        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>HtmlAttribute.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public HtmlAttribute CreateAttribute(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            HtmlAttribute attribute = CreateAttribute(name);
            attribute.Value = value;
            return attribute;
        }

        /// <summary>
        /// Creates the comment.
        /// </summary>
        /// <returns>HtmlCommentNode.</returns>
        public HtmlCommentNode CreateComment()
        {
            return (HtmlCommentNode)CreateNode(HtmlNodeType.Comment);
        }

        /// <summary>
        /// Creates the comment.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <returns>HtmlCommentNode.</returns>
        /// <exception cref="ArgumentNullException">comment</exception>
        public HtmlCommentNode CreateComment(string comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }
            HtmlCommentNode node = CreateComment();
            node.Comment = comment;
            return node;
        }

        /// <summary>
        /// Creates the element.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public HtmlNode CreateElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            HtmlNode node = CreateNode(HtmlNodeType.Element);
            node.Name = name;
            return node;
        }

        /// <summary>
        /// Creates the navigator.
        /// </summary>
        /// <returns>XPathNavigator.</returns>
        public XPathNavigator CreateNavigator()
        {
            return new HtmlNodeNavigator(this, _documentnode);
        }

        internal HtmlNode CreateNode(HtmlNodeType type)
        {
            return CreateNode(type, -1);
        }

        internal HtmlNode CreateNode(HtmlNodeType type, int index)
        {
            switch (type)
            {
                case HtmlNodeType.Comment:
                    return new HtmlCommentNode(this, index);

                case HtmlNodeType.Text:
                    return new HtmlTextNode(this, index);
            }
            return new HtmlNode(type, this, index);
        }

        /// <summary>
        /// Creates the text node.
        /// </summary>
        /// <returns>HtmlTextNode.</returns>
        public HtmlTextNode CreateTextNode()
        {
            return (HtmlTextNode)CreateNode(HtmlNodeType.Text);
        }

        /// <summary>
        /// Creates the text node.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>HtmlTextNode.</returns>
        /// <exception cref="ArgumentNullException">text</exception>
        public HtmlTextNode CreateTextNode(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            HtmlTextNode node = CreateTextNode();
            node.Text = text;
            return node;
        }

        private string CurrentNodeName()
        {
            return Text.Substring(_currentnode._namestartindex, _currentnode._namelength);
        }

        private void DecrementPosition()
        {
            _index--;
            if (_lineposition == 1)
            {
                _lineposition = _maxlineposition;
                _line--;
            }
            else
            {
                _lineposition--;
            }
        }

        /// <summary>
        /// Detects the encoding.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Encoding.</returns>
        /// <exception cref="ArgumentNullException">stream</exception>
        public Encoding DetectEncoding(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            return DetectEncoding(new StreamReader(stream));
        }

        /// <summary>
        /// Detects the encoding.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>Encoding.</returns>
        /// <exception cref="ArgumentNullException">reader</exception>
        public Encoding DetectEncoding(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            _onlyDetectEncoding = true;
            if (OptionCheckSyntax)
            {
                Openednodes = new Dictionary<int, HtmlNode>();
            }
            else
            {
                Openednodes = null;
            }
            if (OptionUseIdAttribute)
            {
                Nodesid = new Dictionary<string, HtmlNode>();
            }
            else
            {
                Nodesid = null;
            }
            StreamReader reader2 = reader as StreamReader;
            if (reader2 != null)
            {
                _streamencoding = reader2.CurrentEncoding;
            }
            else
            {
                _streamencoding = null;
            }
            _declaredencoding = null;
            Text = reader.ReadToEnd();
            _documentnode = CreateNode(HtmlNodeType.Document, 0);
            try
            {
                Parse();
            }
            catch (EncodingFoundException exception)
            {
                return exception.Encoding;
            }
            return null;
        }

        /// <summary>
        /// Detects the encoding.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Encoding.</returns>
        /// <exception cref="ArgumentNullException">path</exception>
        public Encoding DetectEncoding(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            using (StreamReader reader = new StreamReader(path, OptionDefaultStreamEncoding))
            {
                return DetectEncoding(reader);
            }
        }

        /// <summary>
        /// Detects the encoding and load.
        /// </summary>
        /// <param name="path">The path.</param>
        public void DetectEncodingAndLoad(string path)
        {
            DetectEncodingAndLoad(path, true);
        }

        /// <summary>
        /// Detects the encoding and load.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="detectEncoding">if set to <c>true</c> [detect encoding].</param>
        /// <exception cref="ArgumentNullException">path</exception>
        public void DetectEncodingAndLoad(string path, bool detectEncoding)
        {
            Encoding encoding;
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (detectEncoding)
            {
                encoding = DetectEncoding(path);
            }
            else
            {
                encoding = null;
            }
            if (encoding == null)
            {
                Load(path);
            }
            else
            {
                Load(path, encoding);
            }
        }

        /// <summary>
        /// Detects the encoding HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns>Encoding.</returns>
        /// <exception cref="ArgumentNullException">html</exception>
        public Encoding DetectEncodingHtml(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }
            using (StringReader reader = new StringReader(html))
            {
                return DetectEncoding(reader);
            }
        }

        private HtmlNode FindResetterNode(HtmlNode node, string name)
        {
            HtmlNode dictionaryValueOrNull = Utilities.GetDictionaryValueOrNull<string, HtmlNode>(Lastnodes, name);
            if (dictionaryValueOrNull == null)
            {
                return null;
            }
            if (dictionaryValueOrNull.Closed)
            {
                return null;
            }
            if (dictionaryValueOrNull._streamposition < node._streamposition)
            {
                return null;
            }
            return dictionaryValueOrNull;
        }

        private bool FindResetterNodes(HtmlNode node, string[] names)
        {
            if (names != null)
            {
                for (int i = 0; i < names.Length; i++)
                {
                    if (FindResetterNode(node, names[i]) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void FixNestedTag(string name, string[] resetters)
        {
            if (resetters != null)
            {
                HtmlNode dictionaryValueOrNull = Utilities.GetDictionaryValueOrNull<string, HtmlNode>(Lastnodes, _currentnode.Name);
                if (((dictionaryValueOrNull != null) && !Lastnodes[name].Closed) && !FindResetterNodes(dictionaryValueOrNull, resetters))
                {
                    HtmlNode node2 = null;
                    node2 = new HtmlNode(dictionaryValueOrNull.NodeType, this, -1)
                    {
                        _endnode = node2
                    };
                    dictionaryValueOrNull.CloseNode(node2);
                }
            }
        }

        private void FixNestedTags()
        {
            if (_currentnode._starttag)
            {
                string name = CurrentNodeName();
                FixNestedTag(name, GetResetters(name));
            }
        }

        /// <summary>
        /// Gets the elementby identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>HtmlNode.</returns>
        /// <exception cref="ArgumentNullException">id</exception>
        /// <exception cref="Exception"></exception>
        public HtmlNode GetElementbyId(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (Nodesid == null)
            {
                throw new Exception(HtmlExceptionUseIdAttributeFalse);
            }
            if (!Nodesid.ContainsKey(id.ToLower()))
            {
                return null;
            }
            return Nodesid[id.ToLower()];
        }

        internal Encoding GetOutEncoding()
        {
            return (_declaredencoding ?? (_streamencoding ?? OptionDefaultStreamEncoding));
        }

        private string[] GetResetters(string name)
        {
            switch (name)
            {
                case "li":
                    return new string[] { "ul" };

                case "tr":
                    return new string[] { "table" };

                case "th":
                case "td":
                    return new string[] { "tr", "table" };
            }
            return null;
        }

        internal HtmlNode GetXmlDeclaration()
        {
            if (_documentnode.HasChildNodes)
            {
                foreach (HtmlNode node in (IEnumerable<HtmlNode>)_documentnode._childnodes)
                {
                    if (node.Name == "?xml")
                    {
                        return node;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the name of the XML.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public static string GetXmlName(string name)
        {
            string str = string.Empty;
            bool flag = true;
            for (int i = 0; i < name.Length; i++)
            {
                if ((((name[i] >= 'a') && (name[i] <= 'z')) || ((name[i] >= '0') && (name[i] <= '9'))) || (((name[i] == '_') || (name[i] == '-')) || (name[i] == '.')))
                {
                    str = str + name[i];
                }
                else
                {
                    flag = false;
                    byte[] bytes = Encoding.UTF8.GetBytes(new char[] { name[i] });
                    for (int j = 0; j < bytes.Length; j++)
                    {
                        str = str + bytes[j].ToString("x2");
                    }
                    str = str + "_";
                }
            }
            if (flag)
            {
                return str;
            }
            return ("_" + str);
        }

        /// <summary>
        /// HTMLs the encode.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="ArgumentNullException">html</exception>
        public static string HtmlEncode(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }
            Regex regex = new Regex("&(?!(amp;)|(lt;)|(gt;)|(quot;))", RegexOptions.IgnoreCase);
            return regex.Replace(html, "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        private void IncrementPosition()
        {
            if (_crc32 != null)
            {
                _crc32.AddToCRC32(_c);
            }
            _index++;
            _maxlineposition = _lineposition;
            if (_c == 10)
            {
                _lineposition = 1;
                _line++;
            }
            else
            {
                _lineposition++;
            }
        }

        /// <summary>
        /// Determines whether [is white space] [the specified c].
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns><c>true</c> if [is white space] [the specified c]; otherwise, <c>false</c>.</returns>
        public static bool IsWhiteSpace(int c)
        {
            if (((c != 10) && (c != 13)) && ((c != 0x20) && (c != 9)))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Load(Stream stream)
        {
            Load(new StreamReader(stream, OptionDefaultStreamEncoding));
        }

        /// <summary>
        /// Loads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <exception cref="ArgumentNullException">reader</exception>
        public void Load(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            _onlyDetectEncoding = false;
            if (OptionCheckSyntax)
            {
                Openednodes = new Dictionary<int, HtmlNode>();
            }
            else
            {
                Openednodes = null;
            }
            if (OptionUseIdAttribute)
            {
                Nodesid = new Dictionary<string, HtmlNode>();
            }
            else
            {
                Nodesid = null;
            }
            StreamReader reader2 = reader as StreamReader;
            if (reader2 != null)
            {
                try
                {
                    reader2.Peek();
                }
                catch (Exception)
                {
                }
                _streamencoding = reader2.CurrentEncoding;
            }
            else
            {
                _streamencoding = null;
            }
            _declaredencoding = null;
            Text = reader.ReadToEnd();
            _documentnode = CreateNode(HtmlNodeType.Document, 0);
            Parse();
            if (OptionCheckSyntax && (Openednodes != null))
            {
                foreach (HtmlNode node in Openednodes.Values)
                {
                    if (node._starttag)
                    {
                        string outerHtml;
                        if (OptionExtractErrorSourceText)
                        {
                            outerHtml = node.OuterHtml;
                            if (outerHtml.Length > OptionExtractErrorSourceTextMaxLength)
                            {
                                outerHtml = outerHtml.Substring(0, OptionExtractErrorSourceTextMaxLength);
                            }
                        }
                        else
                        {
                            outerHtml = string.Empty;
                        }
                        AddError(HtmlParseErrorCode.TagNotClosed, node._line, node._lineposition, node._streamposition, outerHtml, "End tag </" + node.Name + "> was not found");
                    }
                }
                Openednodes.Clear();
            }
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="ArgumentNullException">path</exception>
        public void Load(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            using (StreamReader reader = new StreamReader(path, OptionDefaultStreamEncoding))
            {
                Load(reader);
            }
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public void Load(Stream stream, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(stream, detectEncodingFromByteOrderMarks));
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        public void Load(Stream stream, Encoding encoding)
        {
            Load(new StreamReader(stream, encoding));
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        /// <exception cref="ArgumentNullException">path</exception>
        public void Load(string path, bool detectEncodingFromByteOrderMarks)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            using (StreamReader reader = new StreamReader(path, detectEncodingFromByteOrderMarks))
            {
                Load(reader);
            }
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="ArgumentNullException">
        /// path
        /// or
        /// encoding
        /// </exception>
        public void Load(string path, Encoding encoding)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            using (StreamReader reader = new StreamReader(path, encoding))
            {
                Load(reader);
            }
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public void Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks));
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        /// <exception cref="ArgumentNullException">
        /// path
        /// or
        /// encoding
        /// </exception>
        public void Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            using (StreamReader reader = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks))
            {
                Load(reader);
            }
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        /// <param name="buffersize">The buffersize.</param>
        public void Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            Load(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, buffersize));
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        /// <param name="buffersize">The buffersize.</param>
        /// <exception cref="ArgumentNullException">
        /// path
        /// or
        /// encoding
        /// </exception>
        public void Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            using (StreamReader reader = new StreamReader(path, encoding, detectEncodingFromByteOrderMarks, buffersize))
            {
                Load(reader);
            }
        }

        /// <summary>
        /// Loads the HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <exception cref="ArgumentNullException">html</exception>
        public void LoadHtml(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }
            using (StringReader reader = new StringReader(html))
            {
                Load(reader);
            }
        }

        private bool NewCheck()
        {
            if (_c != 60)
            {
                return false;
            }
            if ((_index >= Text.Length) || (Text[_index] != '%'))
            {
                if (!PushNodeEnd(_index - 1, true))
                {
                    _index = Text.Length;
                    return true;
                }
                _state = ParseState.WhichTag;
                if (((_index - 1) <= (Text.Length - 2)) && (Text[_index] == '!'))
                {
                    PushNodeStart(HtmlNodeType.Comment, _index - 1);
                    PushNodeNameStart(true, _index);
                    PushNodeNameEnd(_index + 1);
                    _state = ParseState.Comment;
                    if (_index < (Text.Length - 2))
                    {
                        if ((Text[_index + 1] == '-') && (Text[_index + 2] == '-'))
                        {
                            _fullcomment = true;
                        }
                        else
                        {
                            _fullcomment = false;
                        }
                    }
                    return true;
                }
                PushNodeStart(HtmlNodeType.Element, _index - 1);
                return true;
            }
            ParseState state = _state;
            switch (state)
            {
                case ParseState.WhichTag:
                    PushNodeNameStart(true, _index - 1);
                    _state = ParseState.Tag;
                    break;

                case ParseState.Tag:
                    break;

                case ParseState.BetweenAttributes:
                    PushAttributeNameStart(_index - 1);
                    break;

                default:
                    if (state == ParseState.AttributeAfterEquals)
                    {
                        PushAttributeValueStart(_index - 1);
                    }
                    break;
            }
            _oldstate = _state;
            _state = ParseState.ServerSideCode;
            return true;
        }

        private void Parse()
        {
            int num = 0;
            if (OptionComputeChecksum)
            {
                _crc32 = new Crc32();
            }
            Lastnodes = new Dictionary<string, HtmlNode>();
            _c = 0;
            _fullcomment = false;
            _parseerrors = new List<HtmlParseError>();
            _line = 1;
            _lineposition = 1;
            _maxlineposition = 1;
            _state = ParseState.Text;
            _oldstate = _state;
            _documentnode._innerlength = Text.Length;
            _documentnode._outerlength = Text.Length;
            _remainderOffset = Text.Length;
            _lastparentnode = _documentnode;
            _currentnode = CreateNode(HtmlNodeType.Text, 0);
            _currentattribute = null;
            _index = 0;
            PushNodeStart(HtmlNodeType.Text, 0);
            while (_index < Text.Length)
            {
                _c = Text[_index];
                IncrementPosition();
                switch (_state)
                {
                    case ParseState.Text:
                        {
                            if (!NewCheck())
                            {
                            }
                            continue;
                        }
                    case ParseState.WhichTag:
                        if (NewCheck())
                        {
                            continue;
                        }
                        if (_c != 0x2f)
                        {
                            break;
                        }
                        PushNodeNameStart(false, _index);
                        goto Label_017F;

                    case ParseState.Tag:
                        {
                            if (!NewCheck())
                            {
                                if (!IsWhiteSpace(_c))
                                {
                                    goto Label_01C9;
                                }
                                PushNodeNameEnd(_index - 1);
                                if (_state == ParseState.Tag)
                                {
                                    _state = ParseState.BetweenAttributes;
                                }
                            }
                            continue;
                        }
                    case ParseState.BetweenAttributes:
                        {
                            if (!NewCheck() && !IsWhiteSpace(_c))
                            {
                                if ((_c != 0x2f) && (_c != 0x3f))
                                {
                                    goto Label_02A5;
                                }
                                _state = ParseState.EmptyTag;
                            }
                            continue;
                        }
                    case ParseState.EmptyTag:
                        {
                            if (!NewCheck())
                            {
                                if (_c != 0x3e)
                                {
                                    goto Label_0372;
                                }
                                if (PushNodeEnd(_index, true))
                                {
                                    goto Label_034D;
                                }
                                _index = Text.Length;
                            }
                            continue;
                        }
                    case ParseState.AttributeName:
                        {
                            if (!NewCheck())
                            {
                                if (!IsWhiteSpace(_c))
                                {
                                    goto Label_03B0;
                                }
                                PushAttributeNameEnd(_index - 1);
                                _state = ParseState.AttributeBeforeEquals;
                            }
                            continue;
                        }
                    case ParseState.AttributeBeforeEquals:
                        {
                            if (!NewCheck() && !IsWhiteSpace(_c))
                            {
                                if (_c != 0x3e)
                                {
                                    goto Label_04A8;
                                }
                                if (PushNodeEnd(_index, false))
                                {
                                    goto Label_0483;
                                }
                                _index = Text.Length;
                            }
                            continue;
                        }
                    case ParseState.AttributeAfterEquals:
                        {
                            if (!NewCheck() && !IsWhiteSpace(_c))
                            {
                                if ((_c != 0x27) && (_c != 0x22))
                                {
                                    goto Label_0525;
                                }
                                _state = ParseState.QuotedAttributeValue;
                                PushAttributeValueStart(_index, _c);
                                num = _c;
                            }
                            continue;
                        }
                    case ParseState.AttributeValue:
                        {
                            if (!NewCheck())
                            {
                                if (!IsWhiteSpace(_c))
                                {
                                    goto Label_05C5;
                                }
                                PushAttributeValueEnd(_index - 1);
                                _state = ParseState.BetweenAttributes;
                            }
                            continue;
                        }
                    case ParseState.Comment:
                        {
                            if ((_c == 0x3e) && (!_fullcomment || ((Text[_index - 2] == '-') && (Text[_index - 3] == '-'))))
                            {
                                if (!PushNodeEnd(_index, false))
                                {
                                    _index = Text.Length;
                                }
                                else
                                {
                                    _state = ParseState.Text;
                                    PushNodeStart(HtmlNodeType.Text, _index);
                                }
                            }
                            continue;
                        }
                    case ParseState.QuotedAttributeValue:
                        {
                            if (_c != num)
                            {
                                goto Label_064D;
                            }
                            PushAttributeValueEnd(_index - 1);
                            _state = ParseState.BetweenAttributes;
                            continue;
                        }
                    case ParseState.ServerSideCode:
                        {
                            if (((_c != 0x25) || (_index >= Text.Length)) || (Text[_index] != '>'))
                            {
                                continue;
                            }
                            ParseState state2 = _oldstate;
                            if (state2 == ParseState.BetweenAttributes)
                            {
                                goto Label_077E;
                            }
                            if (state2 != ParseState.AttributeAfterEquals)
                            {
                                goto Label_0795;
                            }
                            _state = ParseState.AttributeValue;
                            goto Label_07A1;
                        }
                    case ParseState.PcData:
                        {
                            if (((_currentnode._namelength + 3) <= (Text.Length - (_index - 1))) && (string.Compare(Text.Substring(_index - 1, _currentnode._namelength + 2), "</" + _currentnode.Name, StringComparison.OrdinalIgnoreCase) == 0))
                            {
                                int c = Text[((_index - 1) + 2) + _currentnode.Name.Length];
                                if ((c == 0x3e) || IsWhiteSpace(c))
                                {
                                    HtmlNode newChild = CreateNode(HtmlNodeType.Text, _currentnode._outerstartindex + _currentnode._outerlength);
                                    newChild._outerlength = (_index - 1) - newChild._outerstartindex;
                                    _currentnode.AppendChild(newChild);
                                    PushNodeStart(HtmlNodeType.Element, _index - 1);
                                    PushNodeNameStart(false, (_index - 1) + 2);
                                    _state = ParseState.Tag;
                                    IncrementPosition();
                                }
                            }
                            continue;
                        }
                    default:
                        {
                            continue;
                        }
                }
                PushNodeNameStart(true, _index - 1);
                DecrementPosition();
                Label_017F:
                _state = ParseState.Tag;
                continue;
                Label_01C9:
                if (_c == 0x2f)
                {
                    PushNodeNameEnd(_index - 1);
                    if (_state == ParseState.Tag)
                    {
                        _state = ParseState.EmptyTag;
                    }
                }
                else if (_c == 0x3e)
                {
                    PushNodeNameEnd(_index - 1);
                    if (_state == ParseState.Tag)
                    {
                        if (!PushNodeEnd(_index, false))
                        {
                            _index = Text.Length;
                        }
                        else if (_state == ParseState.Tag)
                        {
                            _state = ParseState.Text;
                            PushNodeStart(HtmlNodeType.Text, _index);
                        }
                    }
                }
                continue;
                Label_02A5:
                if (_c == 0x3e)
                {
                    if (!PushNodeEnd(_index, false))
                    {
                        _index = Text.Length;
                    }
                    else if (_state == ParseState.BetweenAttributes)
                    {
                        _state = ParseState.Text;
                        PushNodeStart(HtmlNodeType.Text, _index);
                    }
                }
                else
                {
                    PushAttributeNameStart(_index - 1);
                    _state = ParseState.AttributeName;
                }
                continue;
                Label_034D:
                if (_state == ParseState.EmptyTag)
                {
                    _state = ParseState.Text;
                    PushNodeStart(HtmlNodeType.Text, _index);
                }
                continue;
                Label_0372:
                _state = ParseState.BetweenAttributes;
                continue;
                Label_03B0:
                if (_c == 0x3d)
                {
                    PushAttributeNameEnd(_index - 1);
                    _state = ParseState.AttributeAfterEquals;
                }
                else if (_c == 0x3e)
                {
                    PushAttributeNameEnd(_index - 1);
                    if (!PushNodeEnd(_index, false))
                    {
                        _index = Text.Length;
                    }
                    else if (_state == ParseState.AttributeName)
                    {
                        _state = ParseState.Text;
                        PushNodeStart(HtmlNodeType.Text, _index);
                    }
                }
                continue;
                Label_0483:
                if (_state == ParseState.AttributeBeforeEquals)
                {
                    _state = ParseState.Text;
                    PushNodeStart(HtmlNodeType.Text, _index);
                }
                continue;
                Label_04A8:
                if (_c == 0x3d)
                {
                    _state = ParseState.AttributeAfterEquals;
                }
                else
                {
                    _state = ParseState.BetweenAttributes;
                    DecrementPosition();
                }
                continue;
                Label_0525:
                if (_c == 0x3e)
                {
                    if (!PushNodeEnd(_index, false))
                    {
                        _index = Text.Length;
                    }
                    else if (_state == ParseState.AttributeAfterEquals)
                    {
                        _state = ParseState.Text;
                        PushNodeStart(HtmlNodeType.Text, _index);
                    }
                }
                else
                {
                    PushAttributeValueStart(_index - 1);
                    _state = ParseState.AttributeValue;
                }
                continue;
                Label_05C5:
                if (_c == 0x3e)
                {
                    PushAttributeValueEnd(_index - 1);
                    if (!PushNodeEnd(_index, false))
                    {
                        _index = Text.Length;
                    }
                    else if (_state == ParseState.AttributeValue)
                    {
                        _state = ParseState.Text;
                        PushNodeStart(HtmlNodeType.Text, _index);
                    }
                }
                continue;
                Label_064D:
                if (((_c == 60) && (_index < Text.Length)) && (Text[_index] == '%'))
                {
                    _oldstate = _state;
                    _state = ParseState.ServerSideCode;
                }
                continue;
                Label_077E:
                PushAttributeNameEnd(_index + 1);
                _state = ParseState.BetweenAttributes;
                goto Label_07A1;
                Label_0795:
                _state = _oldstate;
                Label_07A1:
                IncrementPosition();
            }
            if (_currentnode._namestartindex > 0)
            {
                PushNodeNameEnd(_index);
            }
            PushNodeEnd(_index, false);
            Lastnodes.Clear();
        }

        private void PushAttributeNameEnd(int index)
        {
            _currentattribute._namelength = index - _currentattribute._namestartindex;
            _currentnode.Attributes.Append(_currentattribute);
        }

        private void PushAttributeNameStart(int index)
        {
            _currentattribute = CreateAttribute();
            _currentattribute._namestartindex = index;
            _currentattribute.Line = _line;
            _currentattribute._lineposition = _lineposition;
            _currentattribute._streamposition = index;
        }

        private void PushAttributeValueEnd(int index)
        {
            _currentattribute._valuelength = index - _currentattribute._valuestartindex;
        }

        private void PushAttributeValueStart(int index)
        {
            PushAttributeValueStart(index, 0);
        }

        private void PushAttributeValueStart(int index, int quote)
        {
            _currentattribute._valuestartindex = index;
            if (quote == 0x27)
            {
                _currentattribute.QuoteType = AttributeValueQuote.SingleQuote;
            }
        }

        private bool PushNodeEnd(int index, bool close)
        {
            _currentnode._outerlength = index - _currentnode._outerstartindex;
            if ((_currentnode._nodetype == HtmlNodeType.Text) || (_currentnode._nodetype == HtmlNodeType.Comment))
            {
                if (_currentnode._outerlength > 0)
                {
                    _currentnode._innerlength = _currentnode._outerlength;
                    _currentnode._innerstartindex = _currentnode._outerstartindex;
                    if (_lastparentnode != null)
                    {
                        _lastparentnode.AppendChild(_currentnode);
                    }
                }
            }
            else if (_currentnode._starttag && (_lastparentnode != _currentnode))
            {
                if (_lastparentnode != null)
                {
                    _lastparentnode.AppendChild(_currentnode);
                }
                ReadDocumentEncoding(_currentnode);
                HtmlNode dictionaryValueOrNull = Utilities.GetDictionaryValueOrNull<string, HtmlNode>(Lastnodes, _currentnode.Name);
                _currentnode._prevwithsamename = dictionaryValueOrNull;
                Lastnodes[_currentnode.Name] = _currentnode;
                if ((_currentnode.NodeType == HtmlNodeType.Document) || (_currentnode.NodeType == HtmlNodeType.Element))
                {
                    _lastparentnode = _currentnode;
                }
                if (HtmlNode.IsCDataElement(CurrentNodeName()))
                {
                    _state = ParseState.PcData;
                    return true;
                }
                if (HtmlNode.IsClosedElement(_currentnode.Name) || HtmlNode.IsEmptyElement(_currentnode.Name))
                {
                    close = true;
                }
            }
            if (close || !_currentnode._starttag)
            {
                if (((OptionStopperNodeName != null) && (_remainder == null)) && (string.Compare(_currentnode.Name, OptionStopperNodeName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    _remainderOffset = index;
                    _remainder = Text.Substring(_remainderOffset);
                    CloseCurrentNode();
                    return false;
                }
                CloseCurrentNode();
            }
            return true;
        }

        private void PushNodeNameEnd(int index)
        {
            _currentnode._namelength = index - _currentnode._namestartindex;
            if (OptionFixNestedTags)
            {
                FixNestedTags();
            }
        }

        private void PushNodeNameStart(bool starttag, int index)
        {
            _currentnode._starttag = starttag;
            _currentnode._namestartindex = index;
        }

        private void PushNodeStart(HtmlNodeType type, int index)
        {
            _currentnode = CreateNode(type, index);
            _currentnode._line = _line;
            _currentnode._lineposition = _lineposition;
            if (type == HtmlNodeType.Element)
            {
                _currentnode._lineposition--;
            }
            _currentnode._streamposition = index;
        }

        private void ReadDocumentEncoding(HtmlNode node)
        {
            if ((OptionReadEncoding && (node._namelength == 4)) && (node.Name == "meta"))
            {
                HtmlAttribute attribute = node.Attributes["http-equiv"];
                if ((attribute != null) && (string.Compare(attribute.Value, "content-type", StringComparison.OrdinalIgnoreCase) == 0))
                {
                    HtmlAttribute attribute2 = node.Attributes["content"];
                    if (attribute2 != null)
                    {
                        string nameValuePairsValue = NameValuePairList.GetNameValuePairsValue(attribute2.Value, "charset");
                        if (!string.IsNullOrEmpty(nameValuePairsValue))
                        {
                            if (string.Equals(nameValuePairsValue, "utf8", StringComparison.OrdinalIgnoreCase))
                            {
                                nameValuePairsValue = "utf-8";
                            }
                            try
                            {
                                _declaredencoding = Encoding.GetEncoding(nameValuePairsValue);
                            }
                            catch (ArgumentException)
                            {
                                _declaredencoding = null;
                            }
                            if (_onlyDetectEncoding)
                            {
                                throw new EncodingFoundException(_declaredencoding);
                            }
                            if (((_streamencoding != null) && (_declaredencoding != null)) && (_declaredencoding.WindowsCodePage != _streamencoding.WindowsCodePage))
                            {
                                AddError(HtmlParseErrorCode.CharsetMismatch, _line, _lineposition, _index, node.OuterHtml, "Encoding mismatch between StreamEncoding: " + _streamencoding.WebName + " and DeclaredEncoding: " + _declaredencoding.WebName);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the specified out stream.
        /// </summary>
        /// <param name="outStream">The out stream.</param>
        public void Save(Stream outStream)
        {
            StreamWriter writer = new StreamWriter(outStream, GetOutEncoding());
            Save(writer);
        }

        /// <summary>
        /// Saves the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Save(StreamWriter writer)
        {
            Save((TextWriter)writer);
        }

        /// <summary>
        /// Saves the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <exception cref="ArgumentNullException">writer</exception>
        public void Save(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            DocumentNode.WriteTo(writer);
            writer.Flush();
        }

        /// <summary>
        /// Saves the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Save(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename, false, GetOutEncoding()))
            {
                Save(writer);
            }
        }

        /// <summary>
        /// Saves the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Save(XmlWriter writer)
        {
            DocumentNode.WriteTo(writer);
            writer.Flush();
        }

        /// <summary>
        /// Saves the specified out stream.
        /// </summary>
        /// <param name="outStream">The out stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="ArgumentNullException">
        /// outStream
        /// or
        /// encoding
        /// </exception>
        public void Save(Stream outStream, Encoding encoding)
        {
            if (outStream == null)
            {
                throw new ArgumentNullException("outStream");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            StreamWriter writer = new StreamWriter(outStream, encoding);
            Save(writer);
        }

        /// <summary>
        /// Saves the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="encoding">The encoding.</param>
        /// <exception cref="ArgumentNullException">
        /// filename
        /// or
        /// encoding
        /// </exception>
        public void Save(string filename, Encoding encoding)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            using (StreamWriter writer = new StreamWriter(filename, false, encoding))
            {
                Save(writer);
            }
        }

        internal void SetIdForNode(HtmlNode node, string id)
        {
            if (OptionUseIdAttribute && ((Nodesid != null) && (id != null)))
            {
                if (node == null)
                {
                    Nodesid.Remove(id.ToLower());
                }
                else
                {
                    Nodesid[id.ToLower()] = node;
                }
            }
        }

        internal void UpdateLastParentNode()
        {
            do
            {
                if (_lastparentnode.Closed)
                {
                    _lastparentnode = _lastparentnode.ParentNode;
                }
            }
            while ((_lastparentnode != null) && _lastparentnode.Closed);
            if (_lastparentnode == null)
            {
                _lastparentnode = _documentnode;
            }
        }

        /// <summary>
        /// Gets the check sum.
        /// </summary>
        /// <value>The check sum.</value>
        public int CheckSum
        {
            get
            {
                if (_crc32 != null)
                {
                    return (int)_crc32.CheckSum;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the declared encoding.
        /// </summary>
        /// <value>The declared encoding.</value>
        public Encoding DeclaredEncoding
        {
            get
            {
                return _declaredencoding;
            }
        }

        /// <summary>
        /// Gets the document node.
        /// </summary>
        /// <value>The document node.</value>
        public HtmlNode DocumentNode
        {
            get
            {
                return _documentnode;
            }
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding
        {
            get
            {
                return GetOutEncoding();
            }
        }

        /// <summary>
        /// Gets the parse errors.
        /// </summary>
        /// <value>The parse errors.</value>
        public IEnumerable<HtmlParseError> ParseErrors
        {
            get
            {
                return _parseerrors;
            }
        }

        /// <summary>
        /// Gets the remainder.
        /// </summary>
        /// <value>The remainder.</value>
        public string Remainder
        {
            get
            {
                return _remainder;
            }
        }

        /// <summary>
        /// Gets the remainder offset.
        /// </summary>
        /// <value>The remainder offset.</value>
        public int RemainderOffset
        {
            get
            {
                return _remainderOffset;
            }
        }

        /// <summary>
        /// Gets the stream encoding.
        /// </summary>
        /// <value>The stream encoding.</value>
        public Encoding StreamEncoding
        {
            get
            {
                return _streamencoding;
            }
        }

        private enum ParseState
        {
            /// <summary>
            /// The text
            /// </summary>
            Text,

            /// <summary>
            /// The which tag
            /// </summary>
            WhichTag,

            /// <summary>
            /// The tag
            /// </summary>
            Tag,

            /// <summary>
            /// The between attributes
            /// </summary>
            BetweenAttributes,

            /// <summary>
            /// The empty tag
            /// </summary>
            EmptyTag,

            /// <summary>
            /// The attribute name
            /// </summary>
            AttributeName,

            /// <summary>
            /// The attribute before equals
            /// </summary>
            AttributeBeforeEquals,

            /// <summary>
            /// The attribute after equals
            /// </summary>
            AttributeAfterEquals,

            /// <summary>
            /// The attribute value
            /// </summary>
            AttributeValue,

            /// <summary>
            /// The comment
            /// </summary>
            Comment,

            /// <summary>
            /// The quoted attribute value
            /// </summary>
            QuotedAttributeValue,

            /// <summary>
            /// The server side code
            /// </summary>
            ServerSideCode,

            /// <summary>
            /// The pc data
            /// </summary>
            PcData
        }
    }
}