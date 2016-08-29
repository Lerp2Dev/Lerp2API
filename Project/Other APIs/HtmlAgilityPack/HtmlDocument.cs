namespace HtmlAgilityPack
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.XPath;

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
        public bool OptionAddDebuggingAttributes;
        public bool OptionAutoCloseOnEnd;
        public bool OptionComputeChecksum;
        public bool OptionCheckSyntax = true;
        public Encoding OptionDefaultStreamEncoding;
        public bool OptionExtractErrorSourceText;
        public int OptionExtractErrorSourceTextMaxLength = 100;
        public bool OptionFixNestedTags;
        public bool OptionOutputAsXml;
        public bool OptionOutputOptimizeAttributeValues;
        public bool OptionOutputOriginalCase;
        public bool OptionOutputUpperCase;
        public bool OptionReadEncoding = true;
        public string OptionStopperNodeName;
        public bool OptionUseIdAttribute = true;
        public bool OptionWriteEmptyNodes;
        internal string Text;

        public HtmlDocument()
        {
            this._documentnode = this.CreateNode(HtmlNodeType.Document, 0);
            this.OptionDefaultStreamEncoding = Encoding.Default;
        }

        private void AddError(HtmlParseErrorCode code, int line, int linePosition, int streamPosition, string sourceText, string reason)
        {
            HtmlParseError item = new HtmlParseError(code, line, linePosition, streamPosition, sourceText, reason);
            this._parseerrors.Add(item);
        }

        private void CloseCurrentNode()
        {
            if (!this._currentnode.Closed)
            {
                bool flag = false;
                HtmlNode dictionaryValueOrNull = Utilities.GetDictionaryValueOrNull<string, HtmlNode>(this.Lastnodes, this._currentnode.Name);
                if (dictionaryValueOrNull != null)
                {
                    if (this.OptionFixNestedTags && this.FindResetterNodes(dictionaryValueOrNull, this.GetResetters(this._currentnode.Name)))
                    {
                        this.AddError(HtmlParseErrorCode.EndTagInvalidHere, this._currentnode._line, this._currentnode._lineposition, this._currentnode._streamposition, this._currentnode.OuterHtml, "End tag </" + this._currentnode.Name + "> invalid here");
                        flag = true;
                    }
                    if (!flag)
                    {
                        this.Lastnodes[this._currentnode.Name] = dictionaryValueOrNull._prevwithsamename;
                        dictionaryValueOrNull.CloseNode(this._currentnode);
                    }
                }
                else if (!HtmlNode.IsClosedElement(this._currentnode.Name))
                {
                    if (HtmlNode.CanOverlapElement(this._currentnode.Name))
                    {
                        HtmlNode newChild = this.CreateNode(HtmlNodeType.Text, this._currentnode._outerstartindex);
                        newChild._outerlength = this._currentnode._outerlength;
                        ((HtmlTextNode)newChild).Text = ((HtmlTextNode)newChild).Text.ToLower();
                        if (this._lastparentnode != null)
                        {
                            this._lastparentnode.AppendChild(newChild);
                        }
                    }
                    else if (HtmlNode.IsEmptyElement(this._currentnode.Name))
                    {
                        this.AddError(HtmlParseErrorCode.EndTagNotRequired, this._currentnode._line, this._currentnode._lineposition, this._currentnode._streamposition, this._currentnode.OuterHtml, "End tag </" + this._currentnode.Name + "> is not required");
                    }
                    else
                    {
                        this.AddError(HtmlParseErrorCode.TagNotOpened, this._currentnode._line, this._currentnode._lineposition, this._currentnode._streamposition, this._currentnode.OuterHtml, "Start tag <" + this._currentnode.Name + "> was not found");
                        flag = true;
                    }
                }
                else
                {
                    this._currentnode.CloseNode(this._currentnode);
                    if (this._lastparentnode != null)
                    {
                        HtmlNode node2 = null;
                        Stack<HtmlNode> stack = new Stack<HtmlNode>();
                        for (HtmlNode node3 = this._lastparentnode.LastChild; node3 != null; node3 = node3.PreviousSibling)
                        {
                            if ((node3.Name == this._currentnode.Name) && !node3.HasChildNodes)
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
                                this._lastparentnode.RemoveChild(oldChild);
                                node2.AppendChild(oldChild);
                            }
                        }
                        else
                        {
                            this._lastparentnode.AppendChild(this._currentnode);
                        }
                    }
                }
                if ((!flag && (this._lastparentnode != null)) && (!HtmlNode.IsClosedElement(this._currentnode.Name) || this._currentnode._starttag))
                {
                    this.UpdateLastParentNode();
                }
            }
        }

        internal HtmlAttribute CreateAttribute()
        {
            return new HtmlAttribute(this);
        }

        public HtmlAttribute CreateAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            HtmlAttribute attribute = this.CreateAttribute();
            attribute.Name = name;
            return attribute;
        }

        public HtmlAttribute CreateAttribute(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            HtmlAttribute attribute = this.CreateAttribute(name);
            attribute.Value = value;
            return attribute;
        }

        public HtmlCommentNode CreateComment()
        {
            return (HtmlCommentNode)this.CreateNode(HtmlNodeType.Comment);
        }

        public HtmlCommentNode CreateComment(string comment)
        {
            if (comment == null)
            {
                throw new ArgumentNullException("comment");
            }
            HtmlCommentNode node = this.CreateComment();
            node.Comment = comment;
            return node;
        }

        public HtmlNode CreateElement(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            HtmlNode node = this.CreateNode(HtmlNodeType.Element);
            node.Name = name;
            return node;
        }

        public XPathNavigator CreateNavigator()
        {
            return new HtmlNodeNavigator(this, this._documentnode);
        }

        internal HtmlNode CreateNode(HtmlNodeType type)
        {
            return this.CreateNode(type, -1);
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

        public HtmlTextNode CreateTextNode()
        {
            return (HtmlTextNode)this.CreateNode(HtmlNodeType.Text);
        }

        public HtmlTextNode CreateTextNode(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            HtmlTextNode node = this.CreateTextNode();
            node.Text = text;
            return node;
        }

        private string CurrentNodeName()
        {
            return this.Text.Substring(this._currentnode._namestartindex, this._currentnode._namelength);
        }

        private void DecrementPosition()
        {
            this._index--;
            if (this._lineposition == 1)
            {
                this._lineposition = this._maxlineposition;
                this._line--;
            }
            else
            {
                this._lineposition--;
            }
        }

        public Encoding DetectEncoding(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            return this.DetectEncoding(new StreamReader(stream));
        }

        public Encoding DetectEncoding(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            this._onlyDetectEncoding = true;
            if (this.OptionCheckSyntax)
            {
                this.Openednodes = new Dictionary<int, HtmlNode>();
            }
            else
            {
                this.Openednodes = null;
            }
            if (this.OptionUseIdAttribute)
            {
                this.Nodesid = new Dictionary<string, HtmlNode>();
            }
            else
            {
                this.Nodesid = null;
            }
            StreamReader reader2 = reader as StreamReader;
            if (reader2 != null)
            {
                this._streamencoding = reader2.CurrentEncoding;
            }
            else
            {
                this._streamencoding = null;
            }
            this._declaredencoding = null;
            this.Text = reader.ReadToEnd();
            this._documentnode = this.CreateNode(HtmlNodeType.Document, 0);
            try
            {
                this.Parse();
            }
            catch (EncodingFoundException exception)
            {
                return exception.Encoding;
            }
            return null;
        }

        public Encoding DetectEncoding(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            using (StreamReader reader = new StreamReader(path, this.OptionDefaultStreamEncoding))
            {
                return this.DetectEncoding(reader);
            }
        }

        public void DetectEncodingAndLoad(string path)
        {
            this.DetectEncodingAndLoad(path, true);
        }

        public void DetectEncodingAndLoad(string path, bool detectEncoding)
        {
            Encoding encoding;
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (detectEncoding)
            {
                encoding = this.DetectEncoding(path);
            }
            else
            {
                encoding = null;
            }
            if (encoding == null)
            {
                this.Load(path);
            }
            else
            {
                this.Load(path, encoding);
            }
        }

        public Encoding DetectEncodingHtml(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }
            using (StringReader reader = new StringReader(html))
            {
                return this.DetectEncoding(reader);
            }
        }

        private HtmlNode FindResetterNode(HtmlNode node, string name)
        {
            HtmlNode dictionaryValueOrNull = Utilities.GetDictionaryValueOrNull<string, HtmlNode>(this.Lastnodes, name);
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
                    if (this.FindResetterNode(node, names[i]) != null)
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
                HtmlNode dictionaryValueOrNull = Utilities.GetDictionaryValueOrNull<string, HtmlNode>(this.Lastnodes, this._currentnode.Name);
                if (((dictionaryValueOrNull != null) && !this.Lastnodes[name].Closed) && !this.FindResetterNodes(dictionaryValueOrNull, resetters))
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
            if (this._currentnode._starttag)
            {
                string name = this.CurrentNodeName();
                this.FixNestedTag(name, this.GetResetters(name));
            }
        }

        public HtmlNode GetElementbyId(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (this.Nodesid == null)
            {
                throw new Exception(HtmlExceptionUseIdAttributeFalse);
            }
            if (!this.Nodesid.ContainsKey(id.ToLower()))
            {
                return null;
            }
            return this.Nodesid[id.ToLower()];
        }

        internal Encoding GetOutEncoding()
        {
            return (this._declaredencoding ?? (this._streamencoding ?? this.OptionDefaultStreamEncoding));
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
            if (this._documentnode.HasChildNodes)
            {
                foreach (HtmlNode node in (IEnumerable<HtmlNode>)this._documentnode._childnodes)
                {
                    if (node.Name == "?xml")
                    {
                        return node;
                    }
                }
            }
            return null;
        }

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
            if (this._crc32 != null)
            {
                this._crc32.AddToCRC32(this._c);
            }
            this._index++;
            this._maxlineposition = this._lineposition;
            if (this._c == 10)
            {
                this._lineposition = 1;
                this._line++;
            }
            else
            {
                this._lineposition++;
            }
        }

        public static bool IsWhiteSpace(int c)
        {
            if (((c != 10) && (c != 13)) && ((c != 0x20) && (c != 9)))
            {
                return false;
            }
            return true;
        }

        public void Load(Stream stream)
        {
            this.Load(new StreamReader(stream, this.OptionDefaultStreamEncoding));
        }

        public void Load(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            this._onlyDetectEncoding = false;
            if (this.OptionCheckSyntax)
            {
                this.Openednodes = new Dictionary<int, HtmlNode>();
            }
            else
            {
                this.Openednodes = null;
            }
            if (this.OptionUseIdAttribute)
            {
                this.Nodesid = new Dictionary<string, HtmlNode>();
            }
            else
            {
                this.Nodesid = null;
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
                this._streamencoding = reader2.CurrentEncoding;
            }
            else
            {
                this._streamencoding = null;
            }
            this._declaredencoding = null;
            this.Text = reader.ReadToEnd();
            this._documentnode = this.CreateNode(HtmlNodeType.Document, 0);
            this.Parse();
            if (this.OptionCheckSyntax && (this.Openednodes != null))
            {
                foreach (HtmlNode node in this.Openednodes.Values)
                {
                    if (node._starttag)
                    {
                        string outerHtml;
                        if (this.OptionExtractErrorSourceText)
                        {
                            outerHtml = node.OuterHtml;
                            if (outerHtml.Length > this.OptionExtractErrorSourceTextMaxLength)
                            {
                                outerHtml = outerHtml.Substring(0, this.OptionExtractErrorSourceTextMaxLength);
                            }
                        }
                        else
                        {
                            outerHtml = string.Empty;
                        }
                        this.AddError(HtmlParseErrorCode.TagNotClosed, node._line, node._lineposition, node._streamposition, outerHtml, "End tag </" + node.Name + "> was not found");
                    }
                }
                this.Openednodes.Clear();
            }
        }

        public void Load(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            using (StreamReader reader = new StreamReader(path, this.OptionDefaultStreamEncoding))
            {
                this.Load(reader);
            }
        }

        public void Load(Stream stream, bool detectEncodingFromByteOrderMarks)
        {
            this.Load(new StreamReader(stream, detectEncodingFromByteOrderMarks));
        }

        public void Load(Stream stream, Encoding encoding)
        {
            this.Load(new StreamReader(stream, encoding));
        }

        public void Load(string path, bool detectEncodingFromByteOrderMarks)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            using (StreamReader reader = new StreamReader(path, detectEncodingFromByteOrderMarks))
            {
                this.Load(reader);
            }
        }

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
                this.Load(reader);
            }
        }

        public void Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            this.Load(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks));
        }

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
                this.Load(reader);
            }
        }

        public void Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            this.Load(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, buffersize));
        }

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
                this.Load(reader);
            }
        }

        public void LoadHtml(string html)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }
            using (StringReader reader = new StringReader(html))
            {
                this.Load(reader);
            }
        }

        private bool NewCheck()
        {
            if (this._c != 60)
            {
                return false;
            }
            if ((this._index >= this.Text.Length) || (this.Text[this._index] != '%'))
            {
                if (!this.PushNodeEnd(this._index - 1, true))
                {
                    this._index = this.Text.Length;
                    return true;
                }
                this._state = ParseState.WhichTag;
                if (((this._index - 1) <= (this.Text.Length - 2)) && (this.Text[this._index] == '!'))
                {
                    this.PushNodeStart(HtmlNodeType.Comment, this._index - 1);
                    this.PushNodeNameStart(true, this._index);
                    this.PushNodeNameEnd(this._index + 1);
                    this._state = ParseState.Comment;
                    if (this._index < (this.Text.Length - 2))
                    {
                        if ((this.Text[this._index + 1] == '-') && (this.Text[this._index + 2] == '-'))
                        {
                            this._fullcomment = true;
                        }
                        else
                        {
                            this._fullcomment = false;
                        }
                    }
                    return true;
                }
                this.PushNodeStart(HtmlNodeType.Element, this._index - 1);
                return true;
            }
            ParseState state = this._state;
            switch (state)
            {
                case ParseState.WhichTag:
                    this.PushNodeNameStart(true, this._index - 1);
                    this._state = ParseState.Tag;
                    break;

                case ParseState.Tag:
                    break;

                case ParseState.BetweenAttributes:
                    this.PushAttributeNameStart(this._index - 1);
                    break;

                default:
                    if (state == ParseState.AttributeAfterEquals)
                    {
                        this.PushAttributeValueStart(this._index - 1);
                    }
                    break;
            }
            this._oldstate = this._state;
            this._state = ParseState.ServerSideCode;
            return true;
        }

        private void Parse()
        {
            int num = 0;
            if (this.OptionComputeChecksum)
            {
                this._crc32 = new Crc32();
            }
            this.Lastnodes = new Dictionary<string, HtmlNode>();
            this._c = 0;
            this._fullcomment = false;
            this._parseerrors = new List<HtmlParseError>();
            this._line = 1;
            this._lineposition = 1;
            this._maxlineposition = 1;
            this._state = ParseState.Text;
            this._oldstate = this._state;
            this._documentnode._innerlength = this.Text.Length;
            this._documentnode._outerlength = this.Text.Length;
            this._remainderOffset = this.Text.Length;
            this._lastparentnode = this._documentnode;
            this._currentnode = this.CreateNode(HtmlNodeType.Text, 0);
            this._currentattribute = null;
            this._index = 0;
            this.PushNodeStart(HtmlNodeType.Text, 0);
            while (this._index < this.Text.Length)
            {
                this._c = this.Text[this._index];
                this.IncrementPosition();
                switch (this._state)
                {
                    case ParseState.Text:
                        {
                            if (!this.NewCheck())
                            {
                            }
                            continue;
                        }
                    case ParseState.WhichTag:
                        if (this.NewCheck())
                        {
                            continue;
                        }
                        if (this._c != 0x2f)
                        {
                            break;
                        }
                        this.PushNodeNameStart(false, this._index);
                        goto Label_017F;

                    case ParseState.Tag:
                        {
                            if (!this.NewCheck())
                            {
                                if (!IsWhiteSpace(this._c))
                                {
                                    goto Label_01C9;
                                }
                                this.PushNodeNameEnd(this._index - 1);
                                if (this._state == ParseState.Tag)
                                {
                                    this._state = ParseState.BetweenAttributes;
                                }
                            }
                            continue;
                        }
                    case ParseState.BetweenAttributes:
                        {
                            if (!this.NewCheck() && !IsWhiteSpace(this._c))
                            {
                                if ((this._c != 0x2f) && (this._c != 0x3f))
                                {
                                    goto Label_02A5;
                                }
                                this._state = ParseState.EmptyTag;
                            }
                            continue;
                        }
                    case ParseState.EmptyTag:
                        {
                            if (!this.NewCheck())
                            {
                                if (this._c != 0x3e)
                                {
                                    goto Label_0372;
                                }
                                if (this.PushNodeEnd(this._index, true))
                                {
                                    goto Label_034D;
                                }
                                this._index = this.Text.Length;
                            }
                            continue;
                        }
                    case ParseState.AttributeName:
                        {
                            if (!this.NewCheck())
                            {
                                if (!IsWhiteSpace(this._c))
                                {
                                    goto Label_03B0;
                                }
                                this.PushAttributeNameEnd(this._index - 1);
                                this._state = ParseState.AttributeBeforeEquals;
                            }
                            continue;
                        }
                    case ParseState.AttributeBeforeEquals:
                        {
                            if (!this.NewCheck() && !IsWhiteSpace(this._c))
                            {
                                if (this._c != 0x3e)
                                {
                                    goto Label_04A8;
                                }
                                if (this.PushNodeEnd(this._index, false))
                                {
                                    goto Label_0483;
                                }
                                this._index = this.Text.Length;
                            }
                            continue;
                        }
                    case ParseState.AttributeAfterEquals:
                        {
                            if (!this.NewCheck() && !IsWhiteSpace(this._c))
                            {
                                if ((this._c != 0x27) && (this._c != 0x22))
                                {
                                    goto Label_0525;
                                }
                                this._state = ParseState.QuotedAttributeValue;
                                this.PushAttributeValueStart(this._index, this._c);
                                num = this._c;
                            }
                            continue;
                        }
                    case ParseState.AttributeValue:
                        {
                            if (!this.NewCheck())
                            {
                                if (!IsWhiteSpace(this._c))
                                {
                                    goto Label_05C5;
                                }
                                this.PushAttributeValueEnd(this._index - 1);
                                this._state = ParseState.BetweenAttributes;
                            }
                            continue;
                        }
                    case ParseState.Comment:
                        {
                            if ((this._c == 0x3e) && (!this._fullcomment || ((this.Text[this._index - 2] == '-') && (this.Text[this._index - 3] == '-'))))
                            {
                                if (!this.PushNodeEnd(this._index, false))
                                {
                                    this._index = this.Text.Length;
                                }
                                else
                                {
                                    this._state = ParseState.Text;
                                    this.PushNodeStart(HtmlNodeType.Text, this._index);
                                }
                            }
                            continue;
                        }
                    case ParseState.QuotedAttributeValue:
                        {
                            if (this._c != num)
                            {
                                goto Label_064D;
                            }
                            this.PushAttributeValueEnd(this._index - 1);
                            this._state = ParseState.BetweenAttributes;
                            continue;
                        }
                    case ParseState.ServerSideCode:
                        {
                            if (((this._c != 0x25) || (this._index >= this.Text.Length)) || (this.Text[this._index] != '>'))
                            {
                                continue;
                            }
                            ParseState state2 = this._oldstate;
                            if (state2 == ParseState.BetweenAttributes)
                            {
                                goto Label_077E;
                            }
                            if (state2 != ParseState.AttributeAfterEquals)
                            {
                                goto Label_0795;
                            }
                            this._state = ParseState.AttributeValue;
                            goto Label_07A1;
                        }
                    case ParseState.PcData:
                        {
                            if (((this._currentnode._namelength + 3) <= (this.Text.Length - (this._index - 1))) && (string.Compare(this.Text.Substring(this._index - 1, this._currentnode._namelength + 2), "</" + this._currentnode.Name, StringComparison.OrdinalIgnoreCase) == 0))
                            {
                                int c = this.Text[((this._index - 1) + 2) + this._currentnode.Name.Length];
                                if ((c == 0x3e) || IsWhiteSpace(c))
                                {
                                    HtmlNode newChild = this.CreateNode(HtmlNodeType.Text, this._currentnode._outerstartindex + this._currentnode._outerlength);
                                    newChild._outerlength = (this._index - 1) - newChild._outerstartindex;
                                    this._currentnode.AppendChild(newChild);
                                    this.PushNodeStart(HtmlNodeType.Element, this._index - 1);
                                    this.PushNodeNameStart(false, (this._index - 1) + 2);
                                    this._state = ParseState.Tag;
                                    this.IncrementPosition();
                                }
                            }
                            continue;
                        }
                    default:
                        {
                            continue;
                        }
                }
                this.PushNodeNameStart(true, this._index - 1);
                this.DecrementPosition();
                Label_017F:
                this._state = ParseState.Tag;
                continue;
                Label_01C9:
                if (this._c == 0x2f)
                {
                    this.PushNodeNameEnd(this._index - 1);
                    if (this._state == ParseState.Tag)
                    {
                        this._state = ParseState.EmptyTag;
                    }
                }
                else if (this._c == 0x3e)
                {
                    this.PushNodeNameEnd(this._index - 1);
                    if (this._state == ParseState.Tag)
                    {
                        if (!this.PushNodeEnd(this._index, false))
                        {
                            this._index = this.Text.Length;
                        }
                        else if (this._state == ParseState.Tag)
                        {
                            this._state = ParseState.Text;
                            this.PushNodeStart(HtmlNodeType.Text, this._index);
                        }
                    }
                }
                continue;
                Label_02A5:
                if (this._c == 0x3e)
                {
                    if (!this.PushNodeEnd(this._index, false))
                    {
                        this._index = this.Text.Length;
                    }
                    else if (this._state == ParseState.BetweenAttributes)
                    {
                        this._state = ParseState.Text;
                        this.PushNodeStart(HtmlNodeType.Text, this._index);
                    }
                }
                else
                {
                    this.PushAttributeNameStart(this._index - 1);
                    this._state = ParseState.AttributeName;
                }
                continue;
                Label_034D:
                if (this._state == ParseState.EmptyTag)
                {
                    this._state = ParseState.Text;
                    this.PushNodeStart(HtmlNodeType.Text, this._index);
                }
                continue;
                Label_0372:
                this._state = ParseState.BetweenAttributes;
                continue;
                Label_03B0:
                if (this._c == 0x3d)
                {
                    this.PushAttributeNameEnd(this._index - 1);
                    this._state = ParseState.AttributeAfterEquals;
                }
                else if (this._c == 0x3e)
                {
                    this.PushAttributeNameEnd(this._index - 1);
                    if (!this.PushNodeEnd(this._index, false))
                    {
                        this._index = this.Text.Length;
                    }
                    else if (this._state == ParseState.AttributeName)
                    {
                        this._state = ParseState.Text;
                        this.PushNodeStart(HtmlNodeType.Text, this._index);
                    }
                }
                continue;
                Label_0483:
                if (this._state == ParseState.AttributeBeforeEquals)
                {
                    this._state = ParseState.Text;
                    this.PushNodeStart(HtmlNodeType.Text, this._index);
                }
                continue;
                Label_04A8:
                if (this._c == 0x3d)
                {
                    this._state = ParseState.AttributeAfterEquals;
                }
                else
                {
                    this._state = ParseState.BetweenAttributes;
                    this.DecrementPosition();
                }
                continue;
                Label_0525:
                if (this._c == 0x3e)
                {
                    if (!this.PushNodeEnd(this._index, false))
                    {
                        this._index = this.Text.Length;
                    }
                    else if (this._state == ParseState.AttributeAfterEquals)
                    {
                        this._state = ParseState.Text;
                        this.PushNodeStart(HtmlNodeType.Text, this._index);
                    }
                }
                else
                {
                    this.PushAttributeValueStart(this._index - 1);
                    this._state = ParseState.AttributeValue;
                }
                continue;
                Label_05C5:
                if (this._c == 0x3e)
                {
                    this.PushAttributeValueEnd(this._index - 1);
                    if (!this.PushNodeEnd(this._index, false))
                    {
                        this._index = this.Text.Length;
                    }
                    else if (this._state == ParseState.AttributeValue)
                    {
                        this._state = ParseState.Text;
                        this.PushNodeStart(HtmlNodeType.Text, this._index);
                    }
                }
                continue;
                Label_064D:
                if (((this._c == 60) && (this._index < this.Text.Length)) && (this.Text[this._index] == '%'))
                {
                    this._oldstate = this._state;
                    this._state = ParseState.ServerSideCode;
                }
                continue;
                Label_077E:
                this.PushAttributeNameEnd(this._index + 1);
                this._state = ParseState.BetweenAttributes;
                goto Label_07A1;
                Label_0795:
                this._state = this._oldstate;
                Label_07A1:
                this.IncrementPosition();
            }
            if (this._currentnode._namestartindex > 0)
            {
                this.PushNodeNameEnd(this._index);
            }
            this.PushNodeEnd(this._index, false);
            this.Lastnodes.Clear();
        }

        private void PushAttributeNameEnd(int index)
        {
            this._currentattribute._namelength = index - this._currentattribute._namestartindex;
            this._currentnode.Attributes.Append(this._currentattribute);
        }

        private void PushAttributeNameStart(int index)
        {
            this._currentattribute = this.CreateAttribute();
            this._currentattribute._namestartindex = index;
            this._currentattribute.Line = this._line;
            this._currentattribute._lineposition = this._lineposition;
            this._currentattribute._streamposition = index;
        }

        private void PushAttributeValueEnd(int index)
        {
            this._currentattribute._valuelength = index - this._currentattribute._valuestartindex;
        }

        private void PushAttributeValueStart(int index)
        {
            this.PushAttributeValueStart(index, 0);
        }

        private void PushAttributeValueStart(int index, int quote)
        {
            this._currentattribute._valuestartindex = index;
            if (quote == 0x27)
            {
                this._currentattribute.QuoteType = AttributeValueQuote.SingleQuote;
            }
        }

        private bool PushNodeEnd(int index, bool close)
        {
            this._currentnode._outerlength = index - this._currentnode._outerstartindex;
            if ((this._currentnode._nodetype == HtmlNodeType.Text) || (this._currentnode._nodetype == HtmlNodeType.Comment))
            {
                if (this._currentnode._outerlength > 0)
                {
                    this._currentnode._innerlength = this._currentnode._outerlength;
                    this._currentnode._innerstartindex = this._currentnode._outerstartindex;
                    if (this._lastparentnode != null)
                    {
                        this._lastparentnode.AppendChild(this._currentnode);
                    }
                }
            }
            else if (this._currentnode._starttag && (this._lastparentnode != this._currentnode))
            {
                if (this._lastparentnode != null)
                {
                    this._lastparentnode.AppendChild(this._currentnode);
                }
                this.ReadDocumentEncoding(this._currentnode);
                HtmlNode dictionaryValueOrNull = Utilities.GetDictionaryValueOrNull<string, HtmlNode>(this.Lastnodes, this._currentnode.Name);
                this._currentnode._prevwithsamename = dictionaryValueOrNull;
                this.Lastnodes[this._currentnode.Name] = this._currentnode;
                if ((this._currentnode.NodeType == HtmlNodeType.Document) || (this._currentnode.NodeType == HtmlNodeType.Element))
                {
                    this._lastparentnode = this._currentnode;
                }
                if (HtmlNode.IsCDataElement(this.CurrentNodeName()))
                {
                    this._state = ParseState.PcData;
                    return true;
                }
                if (HtmlNode.IsClosedElement(this._currentnode.Name) || HtmlNode.IsEmptyElement(this._currentnode.Name))
                {
                    close = true;
                }
            }
            if (close || !this._currentnode._starttag)
            {
                if (((this.OptionStopperNodeName != null) && (this._remainder == null)) && (string.Compare(this._currentnode.Name, this.OptionStopperNodeName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    this._remainderOffset = index;
                    this._remainder = this.Text.Substring(this._remainderOffset);
                    this.CloseCurrentNode();
                    return false;
                }
                this.CloseCurrentNode();
            }
            return true;
        }

        private void PushNodeNameEnd(int index)
        {
            this._currentnode._namelength = index - this._currentnode._namestartindex;
            if (this.OptionFixNestedTags)
            {
                this.FixNestedTags();
            }
        }

        private void PushNodeNameStart(bool starttag, int index)
        {
            this._currentnode._starttag = starttag;
            this._currentnode._namestartindex = index;
        }

        private void PushNodeStart(HtmlNodeType type, int index)
        {
            this._currentnode = this.CreateNode(type, index);
            this._currentnode._line = this._line;
            this._currentnode._lineposition = this._lineposition;
            if (type == HtmlNodeType.Element)
            {
                this._currentnode._lineposition--;
            }
            this._currentnode._streamposition = index;
        }

        private void ReadDocumentEncoding(HtmlNode node)
        {
            if ((this.OptionReadEncoding && (node._namelength == 4)) && (node.Name == "meta"))
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
                                this._declaredencoding = Encoding.GetEncoding(nameValuePairsValue);
                            }
                            catch (ArgumentException)
                            {
                                this._declaredencoding = null;
                            }
                            if (this._onlyDetectEncoding)
                            {
                                throw new EncodingFoundException(this._declaredencoding);
                            }
                            if (((this._streamencoding != null) && (this._declaredencoding != null)) && (this._declaredencoding.WindowsCodePage != this._streamencoding.WindowsCodePage))
                            {
                                this.AddError(HtmlParseErrorCode.CharsetMismatch, this._line, this._lineposition, this._index, node.OuterHtml, "Encoding mismatch between StreamEncoding: " + this._streamencoding.WebName + " and DeclaredEncoding: " + this._declaredencoding.WebName);
                            }
                        }
                    }
                }
            }
        }

        public void Save(Stream outStream)
        {
            StreamWriter writer = new StreamWriter(outStream, this.GetOutEncoding());
            this.Save(writer);
        }

        public void Save(StreamWriter writer)
        {
            this.Save((TextWriter)writer);
        }

        public void Save(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            this.DocumentNode.WriteTo(writer);
            writer.Flush();
        }

        public void Save(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename, false, this.GetOutEncoding()))
            {
                this.Save(writer);
            }
        }

        public void Save(XmlWriter writer)
        {
            this.DocumentNode.WriteTo(writer);
            writer.Flush();
        }

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
            this.Save(writer);
        }

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
                this.Save(writer);
            }
        }

        internal void SetIdForNode(HtmlNode node, string id)
        {
            if (this.OptionUseIdAttribute && ((this.Nodesid != null) && (id != null)))
            {
                if (node == null)
                {
                    this.Nodesid.Remove(id.ToLower());
                }
                else
                {
                    this.Nodesid[id.ToLower()] = node;
                }
            }
        }

        internal void UpdateLastParentNode()
        {
            do
            {
                if (this._lastparentnode.Closed)
                {
                    this._lastparentnode = this._lastparentnode.ParentNode;
                }
            }
            while ((this._lastparentnode != null) && this._lastparentnode.Closed);
            if (this._lastparentnode == null)
            {
                this._lastparentnode = this._documentnode;
            }
        }

        public int CheckSum
        {
            get
            {
                if (this._crc32 != null)
                {
                    return (int)this._crc32.CheckSum;
                }
                return 0;
            }
        }

        public Encoding DeclaredEncoding
        {
            get
            {
                return this._declaredencoding;
            }
        }

        public HtmlNode DocumentNode
        {
            get
            {
                return this._documentnode;
            }
        }

        public Encoding Encoding
        {
            get
            {
                return this.GetOutEncoding();
            }
        }

        public IEnumerable<HtmlParseError> ParseErrors
        {
            get
            {
                return this._parseerrors;
            }
        }

        public string Remainder
        {
            get
            {
                return this._remainder;
            }
        }

        public int RemainderOffset
        {
            get
            {
                return this._remainderOffset;
            }
        }

        public Encoding StreamEncoding
        {
            get
            {
                return this._streamencoding;
            }
        }

        private enum ParseState
        {
            Text,
            WhichTag,
            Tag,
            BetweenAttributes,
            EmptyTag,
            AttributeName,
            AttributeBeforeEquals,
            AttributeAfterEquals,
            AttributeValue,
            Comment,
            QuotedAttributeValue,
            ServerSideCode,
            PcData
        }
    }
}