namespace HtmlAgilityPack
{
    using System.Net;
    using System.Security;
    using System.Security.Permissions;

    public class PermissionHelper : IPermissionHelper
    {
        public bool GetIsDnsAvailable()
        {
            return true; //WIP //SecurityManager.IsGranted(new DnsPermission(PermissionState.Unrestricted));
        }

        public bool GetIsRegistryAvailable()
        {
            return SecurityManager.IsGranted(new RegistryPermission(PermissionState.Unrestricted));
        }
    }
}