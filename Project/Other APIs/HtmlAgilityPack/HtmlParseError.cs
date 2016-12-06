namespace HtmlAgilityPack
{
    public class HtmlParseError
    {
        private HtmlParseErrorCode _code;
        private int _line;
        private int _linePosition;
        private string _reason;
        private string _sourceText;
        private int _streamPosition;

        internal HtmlParseError(HtmlParseErrorCode code, int line, int linePosition, int streamPosition, string sourceText, string reason)
        {
            _code = code;
            _line = line;
            _linePosition = linePosition;
            _streamPosition = streamPosition;
            _sourceText = sourceText;
            _reason = reason;
        }

        public HtmlParseErrorCode Code
        {
            get
            {
                return _code;
            }
        }

        public int Line
        {
            get
            {
                return _line;
            }
        }

        public int LinePosition
        {
            get
            {
                return _linePosition;
            }
        }

        public string Reason
        {
            get
            {
                return _reason;
            }
        }

        public string SourceText
        {
            get
            {
                return _sourceText;
            }
        }

        public int StreamPosition
        {
            get
            {
                return _streamPosition;
            }
        }
    }
}