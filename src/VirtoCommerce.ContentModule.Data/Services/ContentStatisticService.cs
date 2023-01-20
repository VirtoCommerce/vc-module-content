using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class ContentStatisticService: IContentStatisticService
    {
        private readonly IBlobContentStorageProviderFactory _blobContentStorageProviderFactory;
        private readonly IContentPathResolver _contentPathResolver;

        public ContentStatisticService(
                IBlobContentStorageProviderFactory blobContentStorageProviderFactory,
                IContentPathResolver contentPathResolver
            )
        {
            _blobContentStorageProviderFactory = blobContentStorageProviderFactory;
            _contentPathResolver = contentPathResolver;
        }

        public async Task<int> GetStorePagesCountAsync(string storeId)
        {
            var (contentStorageProvider, path) = Prepare(storeId, ContentConstants.ContentTypes.Pages);
            var result = await CountContentItemsRecursive(path, contentStorageProvider, ContentConstants.ContentTypes.Blogs);
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
            var folders = await contentStorageProvider.SearchAsync(targetPath, null);
            var result = folders.Results.OfType<BlobFolder>().Count();
            return result;
        }

        private (IBlobContentStorageProvider provider, string targetPath) Prepare(string storeId, string contentType)
        {
            var contentStorageProvider = _blobContentStorageProviderFactory.CreateProvider("");
            var targetPath = _contentPathResolver.GetContentBasePath(contentType, storeId);
            return (contentStorageProvider, targetPath);
        }

        //public async Task<ContentStatistic> GetStoreContentStatsAsync(string storeId)
        //{
        //    var contentStorageProvider = _blobContentStorageProviderFactory.CreateProvider("");
        //    var cacheKey = CacheKey.With(GetType(), "pagesCount", $"content-{storeId}");
        //    var pagesCount = _platformMemoryCache.GetOrCreateExclusive(cacheKey, cacheEntry =>
        //    {
        //        cacheEntry.AddExpirationToken(ContentCacheRegion.CreateChangeToken($"content-{storeId}"));
        //        var path = _contentPathResolver.GetContentBasePath(ContentConstants.ContentTypes.Pages, storeId);
        //        var result = CountContentItemsRecursive(path, contentStorageProvider, ContentConstants.ContentTypes.Blogs);
        //        return result;
        //    });

        //    var themesPath = _contentPathResolver.GetContentBasePath(ContentConstants.ContentTypes.Themes, storeId);
        //    var blogsPath = _contentPathResolver.GetContentBasePath(ContentConstants.ContentTypes.Blogs, storeId);

        //    var storeTask = _storeService.GetByIdAsync(storeId, StoreResponseGroup.DynamicProperties.ToString());
        //    var themesTask = contentStorageProvider.SearchAsync(themesPath, null);
        //    var blogsTask = contentStorageProvider.SearchAsync(blogsPath, null);

        //    await Task.WhenAll(themesTask, blogsTask, storeTask);

        //    var store = storeTask.Result;
        //    var themes = themesTask.Result;
        //    var blogs = blogsTask.Result;

        //    var retVal = new ContentStatistic
        //    {
        //        ActiveThemeName = store.DynamicProperties.FirstOrDefault(x => x.Name == "DefaultThemeName")?.Values?.FirstOrDefault()?.Value.ToString()
        //                ?? ContentConstants.DefaultTheme,
        //        ThemesCount = themes.Results.OfType<BlobFolder>().Count(),
        //        BlogsCount = blogs.Results.OfType<BlobFolder>().Count(),
        //        PagesCount = pagesCount
        //    };
        //    return retVal;
        //}

        private async Task<int> CountContentItemsRecursive(string folderUrl, IBlobStorageProvider blobContentStorageProvider, string excludedFolderName = null)
        {
            var searchResult = await blobContentStorageProvider.SearchAsync(folderUrl, null);

            var folders = searchResult.Results.OfType<BlobFolder>();

            var result = searchResult.TotalCount - folders.Count();
            var children = folders.Where(x => excludedFolderName.IsNullOrEmpty() || !x.Name.EqualsInvariant(excludedFolderName));
            foreach (var child in children)
            {
                var childrenFilesCount = await CountContentItemsRecursive(child.Url, blobContentStorageProvider);
                result += childrenFilesCount;
            }
            return result;
        }
    }
}
