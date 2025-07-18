using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Extensions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Services;

public class ContentFileService(IBlobContentStorageProviderFactory blobContentStorageProviderFactory,
            IContentPathResolver contentPathResolver,
            IContentItemTypeRegistrar contentItemTypeRegistrar)
        : IContentFileService
{
    public async Task<IList<ContentItem>> FilterItemsAsync(FilterItemsCriteria criteria)
    {
        var storageProvider = GetStorageProvider(criteria.ContentType, criteria.StoreId, criteria.FolderUrl);
        var searchResult = await storageProvider.SearchAsync(criteria.FolderUrl ?? "", criteria.Keyword);

        // display folders before files
        var folders = searchResult.Results.OfType<BlobFolder>().Select(x => x.ToContentModel()).OfType<ContentItem>();
        var files = searchResult.Results.OfType<BlobInfo>().Select(x => x.ToContentModel()).OfType<ContentItem>();

        var result = folders
            // Exclude Blogs folder at root level
            .Where(x => criteria.FolderUrl != null || !x.Name.EqualsIgnoreCase(ContentConstants.ContentTypes.Blogs))
            .Concat(files)
            .ToList();

        return result;
    }

    public async Task<IList<ContentFile>> EnumerateFiles(FilterItemsCriteria criteria)
    {
        var storageProvider = GetStorageProvider(criteria.ContentType, criteria.StoreId, criteria.FolderUrl);
        var result = new List<ContentFile>();
        await EnumerateFilesRecursively(storageProvider, criteria.FolderUrl, result);
        return result;
    }

    private IBlobContentStorageProvider GetStorageProvider(string contentType, string storeId, string folderUrl)
    {
        var path = folderUrl != null && new Uri(folderUrl, UriKind.RelativeOrAbsolute).IsAbsoluteUri
            ? ""
            : contentPathResolver.GetContentBasePath(contentType, storeId);
        var storageProvider = blobContentStorageProviderFactory.CreateProvider(path);
        return storageProvider;
    }

    private async Task EnumerateFilesRecursively(IBlobContentStorageProvider storageProvider, string path, IList<ContentFile> result)
    {
        var searchResult = await storageProvider.SearchAsync(path, null);

        var folders = searchResult.Results.OfType<BlobFolder>().Where(x => x.RelativeUrl != path && (path != null || !x.Name.EqualsIgnoreCase(ContentConstants.ContentTypes.Blogs)));
        var files = searchResult.Results.OfType<BlobInfo>()
            .Where(x => contentItemTypeRegistrar.IsRegisteredContentItemType(x.RelativeUrl))
            .Select(x => x.ToContentModel()).ToList();

        result.AddRange(files);

        foreach (var item in folders)
        {
            await EnumerateFilesRecursively(storageProvider, item.RelativeUrl, result);
        }
    }
}
