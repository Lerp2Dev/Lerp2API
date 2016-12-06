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
                if (_code == null)
                {
                    _code = base.FragmentText.Substring(base.Doc.TokenCodeStart.Length, ((base.FragmentText.Length - base.Doc.TokenCodeEnd.Length) - base.Doc.TokenCodeStart.Length) - 1).Trim();
                    if (_code.StartsWith("="))
                    {
                        _code = base.Doc.TokenResponseWrite + _code.Substring(1, _code.Length - 1);
                    }
                }
                return _code;
            }
            set
            {
                _code = value;
            }
        }
    }
}