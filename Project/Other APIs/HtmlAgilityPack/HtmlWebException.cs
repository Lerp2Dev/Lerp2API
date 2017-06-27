namespace HtmlAgilityPack
{
    using System;

    /// <summary>
    /// Class HtmlWebException.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class HtmlWebException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlWebException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public HtmlWebException(string message) : base(message)
        {
        }
    }
}