using Lerp2API.Optimizers;

namespace Lerp2API._Debug
{
    /// <summary>
    /// Class DebugColor.
    /// </summary>
    public class DebugColor
    {
        /// <summary>
        /// Gets the normal.
        /// </summary>
        /// <value>The normal.</value>
        public static Color normal
        {
            get { return new Color(1, 1, 1); }
        }

        /// <summary>
        /// Gets the warning.
        /// </summary>
        /// <value>The warning.</value>
        public static Color warning
        {
            get { return new Color(1, 1, 0); }
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error.</value>
        public static Color error
        {
            get { return new Color(1, 0, 0); }
        }

        /// <summary>
        /// Gets the assert.
        /// </summary>
        /// <value>The assert.</value>
        public static Color assert
        {
            get { return new Color(1, 1, 1); }
        }

        /// <summary>
        /// Gets the assertion.
        /// </summary>
        /// <value>The assertion.</value>
        public static Color assertion
        {
            get { return new Color(0, 1, 1); }
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public static Color exception
        {
            get { return new Color(1, 0, 0); }
        }
    }
}