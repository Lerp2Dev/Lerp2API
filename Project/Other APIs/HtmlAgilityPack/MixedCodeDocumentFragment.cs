namespace HtmlAgilityPack
{
    /// <summary>
    /// Class MixedCodeDocumentFragment.
    /// </summary>
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

        /// <summary>
        /// Gets the fragment text.
        /// </summary>
        /// <value>The fragment text.</value>
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

        /// <summary>
        /// Gets the type of the fragment.
        /// </summary>
        /// <value>The type of the fragment.</value>
        public MixedCodeDocumentFragmentType FragmentType
        {
            get
            {
                return _type;
            }
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
        /// Gets the stream position.
        /// </summary>
        /// <value>The stream position.</value>
        public int StreamPosition
        {
            get
            {
                return Index;
            }
        }
    }
}