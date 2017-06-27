namespace HtmlAgilityPack
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Class MixedCodeDocument.
    /// </summary>
    public class MixedCodeDocument
    {
        private int _c;
        internal MixedCodeDocumentFragmentList _codefragments;
        private MixedCodeDocumentFragment _currentfragment;
        internal MixedCodeDocumentFragmentList _fragments;
        private int _index;
        private int _line;
        private int _lineposition;
        private ParseState _state;
        private Encoding _streamencoding;
        internal string _text;
        internal MixedCodeDocumentFragmentList _textfragments;
        /// <summary>
        /// The token code end
        /// </summary>
        public string TokenCodeEnd = "%>";
        /// <summary>
        /// The token code start
        /// </summary>
        public string TokenCodeStart = "<%";
        /// <summary>
        /// The token directive
        /// </summary>
        public string TokenDirective = "@";
        /// <summary>
        /// The token response write
        /// </summary>
        public string TokenResponseWrite = "Response.Write ";
        private string TokenTextBlock = "TextBlock({0})";

        /// <summary>
        /// Initializes a new instance of the <see cref="MixedCodeDocument"/> class.
        /// </summary>
        public MixedCodeDocument()
        {
            _codefragments = new MixedCodeDocumentFragmentList(this);
            _textfragments = new MixedCodeDocumentFragmentList(this);
            _fragments = new MixedCodeDocumentFragmentList(this);
        }

        /// <summary>
        /// Creates the code fragment.
        /// </summary>
        /// <returns>MixedCodeDocumentCodeFragment.</returns>
        public MixedCodeDocumentCodeFragment CreateCodeFragment()
        {
            return (MixedCodeDocumentCodeFragment)CreateFragment(MixedCodeDocumentFragmentType.Code);
        }

        internal MixedCodeDocumentFragment CreateFragment(MixedCodeDocumentFragmentType type)
        {
            switch (type)
            {
                case MixedCodeDocumentFragmentType.Code:
                    return new MixedCodeDocumentCodeFragment(this);

                case MixedCodeDocumentFragmentType.Text:
                    return new MixedCodeDocumentTextFragment(this);
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// Creates the text fragment.
        /// </summary>
        /// <returns>MixedCodeDocumentTextFragment.</returns>
        public MixedCodeDocumentTextFragment CreateTextFragment()
        {
            return (MixedCodeDocumentTextFragment)CreateFragment(MixedCodeDocumentFragmentType.Text);
        }

        internal Encoding GetOutEncoding()
        {
            if (_streamencoding != null)
            {
                return _streamencoding;
            }
            return Encoding.UTF8;
        }

        private void IncrementPosition()
        {
            _index++;
            if (_c == 10)
            {
                _lineposition = 1;
                _line++;
            }
            else
            {
                _lineposition++;
            }
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void Load(Stream stream)
        {
            Load(new StreamReader(stream));
        }

        /// <summary>
        /// Loads the specified reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void Load(TextReader reader)
        {
            _codefragments.Clear();
            _textfragments.Clear();
            StreamReader reader2 = reader as StreamReader;
            if (reader2 != null)
            {
                _streamencoding = reader2.CurrentEncoding;
            }
            _text = reader.ReadToEnd();
            reader.Close();
            Parse();
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Load(string path)
        {
            Load(new StreamReader(path));
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public void Load(Stream stream, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(stream, detectEncodingFromByteOrderMarks));
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        public void Load(Stream stream, Encoding encoding)
        {
            Load(new StreamReader(stream, encoding));
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public void Load(string path, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(path, detectEncodingFromByteOrderMarks));
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        public void Load(string path, Encoding encoding)
        {
            Load(new StreamReader(path, encoding));
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public void Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks));
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        public void Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(path, encoding, detectEncodingFromByteOrderMarks));
        }

        /// <summary>
        /// Loads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        /// <param name="buffersize">The buffersize.</param>
        public void Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            Load(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, buffersize));
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <param name="detectEncodingFromByteOrderMarks">if set to <c>true</c> [detect encoding from byte order marks].</param>
        /// <param name="buffersize">The buffersize.</param>
        public void Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            Load(new StreamReader(path, encoding, detectEncodingFromByteOrderMarks, buffersize));
        }

        /// <summary>
        /// Loads the HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        public void LoadHtml(string html)
        {
            Load(new StringReader(html));
        }

        private void Parse()
        {
            _state = ParseState.Text;
            _index = 0;
            _currentfragment = CreateFragment(MixedCodeDocumentFragmentType.Text);
            while (_index < _text.Length)
            {
                _c = _text[_index];
                IncrementPosition();
                switch (_state)
                {
                    case ParseState.Text:
                        if (((_index + TokenCodeStart.Length) < _text.Length) && (_text.Substring(_index - 1, TokenCodeStart.Length) == TokenCodeStart))
                        {
                            _state = ParseState.Code;
                            _currentfragment.Length = (_index - 1) - _currentfragment.Index;
                            _currentfragment = CreateFragment(MixedCodeDocumentFragmentType.Code);
                            SetPosition();
                        }
                        break;

                    case ParseState.Code:
                        if (((_index + TokenCodeEnd.Length) < _text.Length) && (_text.Substring(_index - 1, TokenCodeEnd.Length) == TokenCodeEnd))
                        {
                            _state = ParseState.Text;
                            _currentfragment.Length = (_index + TokenCodeEnd.Length) - _currentfragment.Index;
                            _index += TokenCodeEnd.Length;
                            _lineposition += TokenCodeEnd.Length;
                            _currentfragment = CreateFragment(MixedCodeDocumentFragmentType.Text);
                            SetPosition();
                        }
                        break;
                }
            }
            _currentfragment.Length = _index - _currentfragment.Index;
        }

        /// <summary>
        /// Saves the specified out stream.
        /// </summary>
        /// <param name="outStream">The out stream.</param>
        public void Save(Stream outStream)
        {
            StreamWriter writer = new StreamWriter(outStream, GetOutEncoding());
            Save(writer);
        }

        /// <summary>
        /// Saves the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Save(StreamWriter writer)
        {
            Save((TextWriter)writer);
        }

        /// <summary>
        /// Saves the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Save(TextWriter writer)
        {
            writer.Flush();
        }

        /// <summary>
        /// Saves the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Save(string filename)
        {
            StreamWriter writer = new StreamWriter(filename, false, GetOutEncoding());
            Save(writer);
        }

        /// <summary>
        /// Saves the specified out stream.
        /// </summary>
        /// <param name="outStream">The out stream.</param>
        /// <param name="encoding">The encoding.</param>
        public void Save(Stream outStream, Encoding encoding)
        {
            StreamWriter writer = new StreamWriter(outStream, encoding);
            Save(writer);
        }

        /// <summary>
        /// Saves the specified filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="encoding">The encoding.</param>
        public void Save(string filename, Encoding encoding)
        {
            StreamWriter writer = new StreamWriter(filename, false, encoding);
            Save(writer);
        }

        private void SetPosition()
        {
            _currentfragment.Line = _line;
            _currentfragment._lineposition = _lineposition;
            _currentfragment.Index = _index - 1;
            _currentfragment.Length = 0;
        }

        /// <summary>
        /// Gets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code
        {
            get
            {
                string str = "";
                int num = 0;
                foreach (MixedCodeDocumentFragment fragment in _fragments)
                {
                    switch (fragment._type)
                    {
                        case MixedCodeDocumentFragmentType.Code:
                            str = str + ((MixedCodeDocumentCodeFragment)fragment).Code + "\n";
                            break;

                        case MixedCodeDocumentFragmentType.Text:
                            str = str + TokenResponseWrite + string.Format(TokenTextBlock, num) + "\n";
                            num++;
                            break;
                    }
                }
                return str;
            }
        }

        /// <summary>
        /// Gets the code fragments.
        /// </summary>
        /// <value>The code fragments.</value>
        public MixedCodeDocumentFragmentList CodeFragments
        {
            get
            {
                return _codefragments;
            }
        }

        /// <summary>
        /// Gets the fragments.
        /// </summary>
        /// <value>The fragments.</value>
        public MixedCodeDocumentFragmentList Fragments
        {
            get
            {
                return _fragments;
            }
        }

        /// <summary>
        /// Gets the stream encoding.
        /// </summary>
        /// <value>The stream encoding.</value>
        public Encoding StreamEncoding
        {
            get
            {
                return _streamencoding;
            }
        }

        /// <summary>
        /// Gets the text fragments.
        /// </summary>
        /// <value>The text fragments.</value>
        public MixedCodeDocumentFragmentList TextFragments
        {
            get
            {
                return _textfragments;
            }
        }

        private enum ParseState
        {
            /// <summary>
            /// The text
            /// </summary>
            Text,
            /// <summary>
            /// The code
            /// </summary>
            Code
        }
    }
}