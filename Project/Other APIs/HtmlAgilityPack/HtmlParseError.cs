namespace HtmlAgilityPack
{
    /// <summary>
    /// Class HtmlParseError.
    /// </summary>
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

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public HtmlParseErrorCode Code
        {
            get
            {
                return _code;
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
        }

        /// <summary>
        /// Gets the line position.
        /// </summary>
        /// <value>The line position.</value>
        public int LinePosition
        {
            get
            {
                return _linePosition;
            }
        }

        /// <summary>
        /// Gets the reason.
        /// </summary>
        /// <value>The reason.</value>
        public string Reason
        {
            get
            {
                return _reason;
            }
        }

        /// <summary>
        /// Gets the source text.
        /// </summary>
        /// <value>The source text.</value>
        public string SourceText
        {
            get
            {
                return _sourceText;
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
                return _streamPosition;
            }
        }
    }
}