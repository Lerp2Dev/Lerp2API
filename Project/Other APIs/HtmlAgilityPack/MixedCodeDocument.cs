namespace HtmlAgilityPack
{
    using System;
    using System.IO;
    using System.Text;

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
        public string TokenCodeEnd = "%>";
        public string TokenCodeStart = "<%";
        public string TokenDirective = "@";
        public string TokenResponseWrite = "Response.Write ";
        private string TokenTextBlock = "TextBlock({0})";

        public MixedCodeDocument()
        {
            _codefragments = new MixedCodeDocumentFragmentList(this);
            _textfragments = new MixedCodeDocumentFragmentList(this);
            _fragments = new MixedCodeDocumentFragmentList(this);
        }

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

        public void Load(Stream stream)
        {
            Load(new StreamReader(stream));
        }

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

        public void Load(string path)
        {
            Load(new StreamReader(path));
        }

        public void Load(Stream stream, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(stream, detectEncodingFromByteOrderMarks));
        }

        public void Load(Stream stream, Encoding encoding)
        {
            Load(new StreamReader(stream, encoding));
        }

        public void Load(string path, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(path, detectEncodingFromByteOrderMarks));
        }

        public void Load(string path, Encoding encoding)
        {
            Load(new StreamReader(path, encoding));
        }

        public void Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks));
        }

        public void Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
        {
            Load(new StreamReader(path, encoding, detectEncodingFromByteOrderMarks));
        }

        public void Load(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            Load(new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, buffersize));
        }

        public void Load(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int buffersize)
        {
            Load(new StreamReader(path, encoding, detectEncodingFromByteOrderMarks, buffersize));
        }

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

        public void Save(Stream outStream)
        {
            StreamWriter writer = new StreamWriter(outStream, GetOutEncoding());
            Save(writer);
        }

        public void Save(StreamWriter writer)
        {
            Save((TextWriter)writer);
        }

        public void Save(TextWriter writer)
        {
            writer.Flush();
        }

        public void Save(string filename)
        {
            StreamWriter writer = new StreamWriter(filename, false, GetOutEncoding());
            Save(writer);
        }

        public void Save(Stream outStream, Encoding encoding)
        {
            StreamWriter writer = new StreamWriter(outStream, encoding);
            Save(writer);
        }

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

        public MixedCodeDocumentFragmentList CodeFragments
        {
            get
            {
                return _codefragments;
            }
        }

        public MixedCodeDocumentFragmentList Fragments
        {
            get
            {
                return _fragments;
            }
        }

        public Encoding StreamEncoding
        {
            get
            {
                return _streamencoding;
            }
        }

        public MixedCodeDocumentFragmentList TextFragments
        {
            get
            {
                return _textfragments;
            }
        }

        private enum ParseState
        {
            Text,
            Code
        }
    }
}