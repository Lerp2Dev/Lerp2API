namespace HtmlAgilityPack
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Class HtmlAttribute.
    /// </summary>
    /// <seealso cref="System.IComparable" />
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

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>HtmlAttribute.</returns>
        public HtmlAttribute Clone()
        {
            return new HtmlAttribute(_ownerdocument) { Name = Name, Value = Value };
        }

        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="ArgumentException">obj</exception>
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

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            _ownernode.Attributes.Remove(this);
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
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        /// <exception cref="ArgumentNullException">value</exception>
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
        /// Gets the owner document.
        /// </summary>
        /// <value>The owner document.</value>
        public HtmlDocument OwnerDocument
        {
            get
            {
                return _ownerdocument;
            }
        }

        /// <summary>
        /// Gets the owner node.
        /// </summary>
        /// <value>The owner node.</value>
        public HtmlNode OwnerNode
        {
            get
            {
                return _ownernode;
            }
        }

        /// <summary>
        /// Gets or sets the type of the quote.
        /// </summary>
        /// <value>The type of the quote.</value>
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
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
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

        /// <summary>
        /// Gets the x path.
        /// </summary>
        /// <value>The x path.</value>
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