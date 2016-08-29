namespace HtmlAgilityPack
{
    public class MixedCodeDocumentCodeFragment : MixedCodeDocumentFragment
    {
        private string _code;

        internal MixedCodeDocumentCodeFragment(MixedCodeDocument doc) : base(doc, MixedCodeDocumentFragmentType.Code)
        {
        }

        public string Code
        {
            get
            {
                if (this._code == null)
                {
                    this._code = base.FragmentText.Substring(base.Doc.TokenCodeStart.Length, ((base.FragmentText.Length - base.Doc.TokenCodeEnd.Length) - base.Doc.TokenCodeStart.Length) - 1).Trim();
                    if (this._code.StartsWith("="))
                    {
                        this._code = base.Doc.TokenResponseWrite + this._code.Substring(1, this._code.Length - 1);
                    }
                }
                return this._code;
            }
            set
            {
                this._code = value;
            }
        }
    }
}