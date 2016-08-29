namespace HtmlAgilityPack
{
    internal class Trace
    {
        internal static Trace _current;

        public static void WriteLine(string message, string category)
        {
            Current.WriteLineIntern(message, category);
        }

        private void WriteLineIntern(string message, string category)
        {
        }

        internal static Trace Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new Trace();
                }
                return _current;
            }
        }
    }
}