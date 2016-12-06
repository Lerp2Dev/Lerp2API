namespace HtmlAgilityPack
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("Name: {OriginalName}, Value: {Value}")]
    public class HtmlAttribute : IComparable
    {
        private int _line;
        internal int _lineposition;
        internal string _name;
        internal int _namelength;
        internal int _namestartindex;
        internal HtmlDocument _ownerdocument;
        internal HtmlNode _ownernode;
        private AttributeValueQuote _quoteType = AttributeValueQuote.DoubleQuote;
        internal int _streamposition;
        internal string _value;
        internal int _valuelength;
        internal int _valuestartindex;

        internal HtmlAttribute(HtmlDocument ownerdocument)
        {
            _ownerdocument = ownerdocument;
        }

        public HtmlAttribute Clone()
        {
            return new HtmlAttribute(_ownerdocument) { Name = Name, Value = Value };
        }

        public int CompareTo(object obj)
        {
            HtmlAttribute attribute = obj as HtmlAttribute;
            if (attribute == null)
            {
                throw new ArgumentException("obj");
            }
            return Name.CompareTo(attribute.Name);
        }

        private string GetRelativeXpath()
        {
            if (OwnerNode == null)
            {
                return Name;
            }
            int num = 1;
            foreach (HtmlAttribute attribute in (IEnumerable<HtmlAttribute>)OwnerNode.Attributes)
            {
                if (!(attribute.Name != Name))
                {
                    if (attribute == this)
                    {
                        break;
                    }
                    num++;
                }
            }
            return string.Concat(new object[] { "@", Name, "[", num, "]" });
        }

        public void Remove()
        {
            _ownernode.Attributes.Remove(this);
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
        }

        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = _ownerdocument.Text.Substring(_namestartindex, _namelength);
                }
                return _name.ToLower();
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _name = value;
                if (_ownernode != null)
                {
                    _ownernode._innerchanged = true;
                    _ownernode._outerchanged = true;
                }
            }
        }

        public string OriginalName
        {
            get
            {
                return _name;
            }
        }

        public HtmlDocument OwnerDocument
        {
            get
            {
                return _ownerdocument;
            }
        }

        public HtmlNode OwnerNode
        {
            get
            {
                return _ownernode;
            }
        }

        public AttributeValueQuote QuoteType
        {
            get
            {
                return _quoteType;
            }
            set
            {
                _quoteType = value;
            }
        }

        public int StreamPosition
        {
            get
            {
                return _streamposition;
            }
        }

        public string Value
        {
            get
            {
                if (_value == null)
                {
                    _value = _ownerdocument.Text.Substring(_valuestartindex, _valuelength);
                }
                return _value;
            }
            set
            {
                _value = value;
                if (_ownernode != null)
                {
                    _ownernode._innerchanged = true;
                    _ownernode._outerchanged = true;
                }
            }
        }

        internal string XmlName
        {
            get
            {
                return HtmlDocument.GetXmlName(Name);
            }
        }

        internal string XmlValue
        {
            get
            {
                return Value;
            }
        }

        public string XPath
        {
            get
            {
                string str = (OwnerNode == null) ? "/" : (OwnerNode.XPath + "/");
                return (str + GetRelativeXpath());
            }
        }
    }
}