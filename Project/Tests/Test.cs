namespace Lerp2API.Tests
{
    /// <summary>
    /// Class Test.
    /// </summary>
    public class Test
    {
        private static string _hola = "";
        /// <summary>
        /// Gets or sets the hola.
        /// </summary>
        /// <value>The hola.</value>
        public static string Hola
        {
            get
            {
                return _hola;
            }
            set
            {
                _hola = value + "bbbb";
            }
        }

        //public static string Hola = "asdj";

        /// <summary>
        /// Sumas the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Int32.</returns>
        public int Suma(int a, int b)
        {
            return a + b;
        }

        /// <summary>
        /// Restas the specified a.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Int32.</returns>
        public static int Resta(int a, int b)
        {
            return a - b;
        }
    }
}
