namespace HtmlAgilityPack
{
    using System;

    internal class EncodingFoundException : Exception
    {
        private System.Text.Encoding _encoding;

        internal EncodingFoundException(System.Text.Encoding encoding)
        {
            _encoding = encoding;
        }

        internal System.Text.Encoding Encoding
        {
            get
            {
                return _encoding;
            }
        }
    }
}