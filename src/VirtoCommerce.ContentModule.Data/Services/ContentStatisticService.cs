using System;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class ContentStatisticService : IContentStatisticService
    {
        private readonly IBlobContentStorageProviderFactory _blobContentStorageProviderFactory;
        private readonly IContentPathResolver _contentPathResolver;

        public ContentStatisticService(
            IBlobContentStorageProviderFactory blobContentStorageProviderFactory,
            IContentPathResolver contentPathResolver)
        {
            _blobContentStorageProviderFactory = blobContentStorageProviderFactory;
            _contentPathResolver = contentPathResolver;
        }

        public async Task<int> GetStorePagesCountAsync(string storeId)
        {
            var (contentStorageProvider, path) = Prepare(storeId, ContentConstants.ContentTypes.Pages);
            var result = await CountContentItemsRecursive(null, contentStorageProvider, null, null, ContentConstants.ContentTypes.Blogs);
            return result;
        }

        public async Task<int> GetStoreChangedPagesCountAsync(string storeId, DateTime? startDate, DateTime? endDate)
        {
            var (contentStorageProvider, path) = Prepare(storeId, ContentConstants.ContentTypes.Pages);
            var result = await CountContentItemsRecursive(null, contentStorageProvider, startDate, endDate);
            return result;
        }

        public async Task<int> GetStoreThemesCountAsync(string storeId)
        {
            var result = await GetFoldersCount(storeId, ContentConstants.ContentTypes.Themes);
            return result;
        }

        public async Task<int> GetStoreBlogsCountAsync(string storeId)
        {
            var result = await GetFoldersCount(storeId, ContentConstants.ContentTypes.Blogs);
            return result;
        }

        private async Task<int> GetFoldersCount(string storeId, string contentType)
        {
            var (contentStorageProvider, targetPath) = Prepare(storeId, contentType);
            var folders = await contentStorageProvider.SearchAsync(null, null);
            var result = folders.Results.OfType<BlobFolder>().Count();
            return result;
        }

        private (IBlobContentStorageProvider provider, string targetPath) Prepare(string storeId, string contentType)
        {
            var targetPath = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var contentStorageProvider = _blobContentStorageProviderFactory.CreateProvider(targetPath);
            return (contentStorageProvider, targetPath);
        }

        private async Task<int> CountContentItemsRecursive(string folderUrl, IBlobStorageProvider blobContentStorageProvider, DateTime? startDate, DateTime? endDate, string excludedFolderName = null)
        {
            var searchResult = await blobContentStorageProvider.SearchAsync(folderUrl, null);
            var folders = searchResult.Results.OfType<BlobFolder>();
            var blobs = searchResult.Results.OfType<BlobInfo>();

            var result = blobs.Count(x => (startDate == null || x.ModifiedDate >= startDate) && (endDate == null || x.ModifiedDate <= endDate));
            var children = folders.Where(x => excludedFolderName.IsNullOrEmpty() || !x.Name.EqualsInvariant(excludedFolderName));

            foreach (var child in children)
            {
                var childrenFilesCount = await CountContentItemsRecursive(child.Url, blobContentStorageProvider, startDate, endDate, excludedFolderName);
                result += childrenFilesCount;
            }

            return result;
        }
    }
}
