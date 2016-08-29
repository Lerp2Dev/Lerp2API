namespace HtmlAgilityPack
{
    public class HtmlTextNode : HtmlNode
    {
        private string _text;

        internal HtmlTextNode(HtmlDocument ownerdocument, int index) : base(HtmlNodeType.Text, ownerdocument, index)
        {
        }

        public override string InnerHtml
        {
            get
            {
                return this.OuterHtml;
            }
            set
            {
                this._text = value;
            }
        }

        public override string OuterHtml
        {
            get
            {
                if (this._text == null)
                {
                    return base.OuterHtml;
                }
                return this._text;
            }
        }

        public string Text
        {
            get
            {
                if (this._text == null)
                {
                    return base.OuterHtml;
                }
                return this._text;
            }
            set
            {
                this._text = value;
            }
        }
    }
}