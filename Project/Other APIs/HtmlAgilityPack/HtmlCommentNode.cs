namespace HtmlAgilityPack
{
    /// <summary>
    /// Class HtmlCommentNode.
    /// </summary>
    /// <seealso cref="HtmlAgilityPack.HtmlNode" />
    public class HtmlCommentNode : HtmlNode
    {
        private string _comment;

        internal HtmlCommentNode(HtmlDocument ownerdocument, int index) : base(HtmlNodeType.Comment, ownerdocument, index)
        {
        }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment
        {
            get
            {
                if (_comment == null)
                {
                    return base.InnerHtml;
                }
                return _comment;
            }
            set
            {
                _comment = value;
            }
        }

        /// <summary>
        /// Gets or sets the inner HTML.
        /// </summary>
        /// <value>The inner HTML.</value>
        public override string InnerHtml
        {
            get
            {
                if (_comment == null)
                {
                    return base.InnerHtml;
                }
                return _comment;
            }
            set
            {
                _comment = value;
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
                if (_comment == null)
                {
                    return base.OuterHtml;
                }
                return ("<!--" + _comment + "-->");
            }
        }
    }
}