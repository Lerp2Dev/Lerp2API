using System.Collections.ObjectModel;
using System.Linq;

namespace System
{
    /// <summary>
    /// Class AggregateException.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class AggregateException : Exception
    {
        /// <summary>
        /// Gets the inner exceptions.
        /// </summary>
        /// <value>The inner exceptions.</value>
        public ReadOnlyCollection<Exception> InnerExceptions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateException"/> class.
        /// </summary>
        /// <param name="exceptions">The exceptions.</param>
        public AggregateException(params Exception[] exceptions) : base("", exceptions.FirstOrDefault())
        {
            InnerExceptions = new ReadOnlyCollection<Exception>(exceptions);
        }
    }
}