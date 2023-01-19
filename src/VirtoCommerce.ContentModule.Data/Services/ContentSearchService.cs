using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Extensions;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.Platform.Core.Common;
using System.Collections;
using System.Collections.Generic;
using StackExchange.Redis;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class ContentSearchService : IContentSearchService
    {
        private readonly IBlobContentStorageProviderFactory _blobContentStorageProviderFactory;
        private readonly IContentPathResolver _contentPathResolver;

        public ContentSearchService(
                IBlobContentStorageProviderFactory blobContentStorageProviderFactory,
                IContentPathResolver contentPathResolver
            )
        {
            _blobContentStorageProviderFactory = blobContentStorageProviderFactory;
            _contentPathResolver = contentPathResolver;
        }

        public async Task<ContentItem[]> FilterContentAsync(string contentType, string storeId, string folderUrl, string keyword)
        {
            var path = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);

            var searchResult = await storageProvider.SearchAsync(folderUrl, keyword);

            var result = searchResult.Results.OfType<BlobFolder>()
                .Select(x => x.ToContentModel())
                .OfType<ContentItem>()
                .Concat(searchResult.Results.OfType<BlobInfo>().Select(x => x.ToContentModel()))
                // todo: question: here we exclude files that have "blogs" in name. It's wrong!
                .Where(x => folderUrl != null || !x.Name.EqualsInvariant(ContentConstants.ContentTypes.Blogs))
                .ToArray();

            return result;
        }

        public async Task<IEnumerable<ContentFile>> EnumerateItems(string contentType, string storeId, string folderUrl)
        {
            var path = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);

            var result = await EnumerateItemsRecursively(storageProvider, folderUrl);
            return result;
        }

        private async Task<IEnumerable<ContentFile>> EnumerateItemsRecursively(IBlobContentStorageProvider storageProvider, string path)
        {
            var searchResult = await storageProvider.SearchAsync(path, null);

            var folders = searchResult.Results.OfType<BlobFolder>();
            var result = searchResult.Results.OfType<BlobInfo>()
                .Select(x => x.ToContentModel()).ToList();

            foreach (var item in folders)
            {
                var children = await EnumerateItemsRecursively(storageProvider, item.Url);
                result.AddRange(children);
            }

            return result;
        }
    }
}
