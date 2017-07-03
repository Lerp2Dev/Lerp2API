namespace UnityInputConverter.YamlDotNet.Samples.Helpers
{
    /// <summary>
    /// Interface ITestOutputHelper
    /// </summary>
    public interface ITestOutputHelper
    {
        /// <summary>
        /// Writes the line.
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="value">The value.</param>
        void WriteLine(string value);

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void WriteLine(string format, params object[] args);
    }
}