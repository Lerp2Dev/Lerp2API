namespace System
{
    /// <summary>
    /// Interface IProgress
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProgress<in T>
    {
        /// <summary>
        /// Reports the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        void Report(T value);
    }
}