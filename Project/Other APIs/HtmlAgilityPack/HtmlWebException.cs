namespace HtmlAgilityPack
{
    using System;

    public class HtmlWebException : Exception
    {
        public HtmlWebException(string message) : base(message)
        {
        }
    }
}