using VirtoCommerce.Platform.Core.Assets;

namespace VirtoCommerce.ContentModule.Data.Services
{
    /// <summary>
    /// Represent functionality to  cms blob content access 
    /// </summary>
    public interface IContentBlobStorageProvider : IBlobStorageProvider
    {
        void MoveContent(string srcUrl, string destUrl);
        void CopyContent(string srcUrl, string destUrl);
    }
}
