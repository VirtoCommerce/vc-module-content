using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class ContentStatisticService: IContentStatisticService
    {
        private readonly IBlobContentStorageProviderFactory _blobContentStorageProviderFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly ICrudService<Store> _storeService;
        private readonly IContentPathResolver _contentFolderResolver;

        public ContentStatisticService(
                IBlobContentStorageProviderFactory blobContentStorageProviderFactory,
                IPlatformMemoryCache platformMemoryCache,
                IStoreService storeService,
                IContentPathResolver contentFolderResolver
            )
        {
            _blobContentStorageProviderFactory = blobContentStorageProviderFactory;
            _platformMemoryCache = platformMemoryCache;
            _storeService = (ICrudService<Store>)storeService;
            _contentFolderResolver = contentFolderResolver;
        }

        public async Task<ContentStatistic> GetStoreContentStatsAsync(string storeId)
        {
            var contentStorageProvider = _blobContentStorageProviderFactory.CreateProvider("");
            var cacheKey = CacheKey.With(GetType(), "pagesCount", $"content-{storeId}");
            var pagesCount = _platformMemoryCache.GetOrCreateExclusive(cacheKey, cacheEntry =>
            {
                cacheEntry.AddExpirationToken(ContentCacheRegion.CreateChangeToken($"content-{storeId}"));
                var path = _contentFolderResolver.GetContentBasePath(ContentConstants.ContentTypes.Pages, storeId);
                var result = CountContentItemsRecursive(path, contentStorageProvider, ContentConstants.ContentTypes.Blogs);
                return result;
            });

            var themesPath = _contentFolderResolver.GetContentBasePath(ContentConstants.ContentTypes.Themes, storeId);
            var blogsPath = _contentFolderResolver.GetContentBasePath(ContentConstants.ContentTypes.Blogs, storeId);

            var storeTask = _storeService.GetByIdAsync(storeId, StoreResponseGroup.DynamicProperties.ToString());
            var themesTask = contentStorageProvider.SearchAsync(themesPath, null);
            var blogsTask = contentStorageProvider.SearchAsync(blogsPath, null);

            await Task.WhenAll(themesTask, blogsTask, storeTask);

            var store = storeTask.Result;
            var themes = themesTask.Result;
            var blogs = blogsTask.Result;

            var retVal = new ContentStatistic
            {
                ActiveThemeName = store.DynamicProperties.FirstOrDefault(x => x.Name == "DefaultThemeName")?.Values?.FirstOrDefault()?.Value.ToString()
                        ?? ContentConstants.DefaultTheme,
                ThemesCount = themes.Results.OfType<BlobFolder>().Count(),
                BlogsCount = blogs.Results.OfType<BlobFolder>().Count(),
                PagesCount = pagesCount
            };
            return retVal;
        }

        private static int CountContentItemsRecursive(string folderUrl, IBlobStorageProvider blobContentStorageProvider, string excludedFolderName = null)
        {
            var searchResult = blobContentStorageProvider.SearchAsync(folderUrl, null).GetAwaiter().GetResult();

            var folders = searchResult.Results.OfType<BlobFolder>();

            var retVal = searchResult.TotalCount - folders.Count()
                         + searchResult.Results.OfType<BlobFolder>()
                             .Where(x => excludedFolderName.IsNullOrEmpty() || !x.Name.EqualsInvariant(excludedFolderName))
                             .Select(x => CountContentItemsRecursive(x.RelativeUrl, blobContentStorageProvider))
                             .Sum();

            return retVal;
        }
    }
}
