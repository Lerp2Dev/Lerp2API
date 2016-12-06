namespace HtmlAgilityPack
{
    public abstract class MixedCodeDocumentFragment
    {
        private string _fragmentText;
        private int _line;
        internal int _lineposition;
        internal MixedCodeDocumentFragmentType _type;
        internal MixedCodeDocument Doc;
        internal int Index;
        internal int Length;

        internal MixedCodeDocumentFragment(MixedCodeDocument doc, MixedCodeDocumentFragmentType type)
        {
            Doc = doc;
            _type = type;
            switch (type)
            {
                case MixedCodeDocumentFragmentType.Code:
                    Doc._codefragments.Append(this);
                    break;

                case MixedCodeDocumentFragmentType.Text:
                    Doc._textfragments.Append(this);
                    break;
            }
            Doc._fragments.Append(this);
        }

        public string FragmentText
        {
            get
            {
                if (_fragmentText == null)
                {
                    _fragmentText = Doc._text.Substring(Index, Length);
                }
                return FragmentText;
            }
            internal set
            {
                _fragmentText = value;
            }
        }

        public MixedCodeDocumentFragmentType FragmentType
        {
            get
            {
                return _type;
            }
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

        public int StreamPosition
        {
            get
            {
                return Index;
            }
        }
    }
}