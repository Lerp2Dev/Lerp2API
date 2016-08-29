namespace HtmlAgilityPack
{
    public interface IPermissionHelper
    {
        bool GetIsDnsAvailable();

        bool GetIsRegistryAvailable();
    }
}