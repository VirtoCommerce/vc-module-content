namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IContentPathResolver
    {
        string GetContentBasePath(string contentType, string storeId, string themeName = null);
    }
}
