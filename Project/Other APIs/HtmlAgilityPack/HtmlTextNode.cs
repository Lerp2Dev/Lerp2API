namespace HtmlAgilityPack
{
    /// <summary>
    /// Class HtmlTextNode.
    /// </summary>
    /// <seealso cref="HtmlAgilityPack.HtmlNode" />
    public class HtmlTextNode : HtmlNode
    {
        private string _text;

        internal HtmlTextNode(HtmlDocument ownerdocument, int index) : base(HtmlNodeType.Text, ownerdocument, index)
        {
        }

        /// <summary>
        /// Gets or sets the inner HTML.
        /// </summary>
        /// <value>The inner HTML.</value>
        public override string InnerHtml
        {
            get
            {
                return OuterHtml;
            }
            set
            {
                _text = value;
            }
        }

        /// <summary>
        /// Gets the outer HTML.
        /// </summary>
        /// <value>The outer HTML.</value>
        public override string OuterHtml
        {
            get
            {
                if (_text == null)
                {
                    return base.OuterHtml;
                }
                return _text;
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                if (_text == null)
                {
                    return base.OuterHtml;
                }
                return _text;
            }
            set
            {
                _text = value;
            }
        }
    }
}