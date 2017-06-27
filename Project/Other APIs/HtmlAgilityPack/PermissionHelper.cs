namespace HtmlAgilityPack
{
    using System.Security;
    using System.Security.Permissions;

    /// <summary>
    /// Class PermissionHelper.
    /// </summary>
    /// <seealso cref="HtmlAgilityPack.IPermissionHelper" />
    public class PermissionHelper : IPermissionHelper
    {
        /// <summary>
        /// Gets the is DNS available.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GetIsDnsAvailable()
        {
            return true; //WIP //SecurityManager.IsGranted(new DnsPermission(PermissionState.Unrestricted));
        }

        /// <summary>
        /// Gets the is registry available.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool GetIsRegistryAvailable()
        {
            return SecurityManager.IsGranted(new RegistryPermission(PermissionState.Unrestricted));
        }
    }
}