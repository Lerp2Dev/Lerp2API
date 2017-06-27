namespace HtmlAgilityPack
{
    /// <summary>
    /// Interface IPermissionHelper
    /// </summary>
    public interface IPermissionHelper
    {
        /// <summary>
        /// Gets the is DNS available.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool GetIsDnsAvailable();

        /// <summary>
        /// Gets the is registry available.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool GetIsRegistryAvailable();
    }
}