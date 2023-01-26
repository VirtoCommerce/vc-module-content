using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Extensions;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class ContentFileService : IContentFileService
    {
        private readonly IBlobContentStorageProviderFactory _blobContentStorageProviderFactory;
        private readonly IContentPathResolver _contentPathResolver;

        public ContentFileService(
                IBlobContentStorageProviderFactory blobContentStorageProviderFactory,
                IContentPathResolver contentPathResolver
            )
        {
            _blobContentStorageProviderFactory = blobContentStorageProviderFactory;
            _contentPathResolver = contentPathResolver;
        }

        public async Task<IList<ContentItem>> FilterFilesAsync(FilterFilesCriteria criteria)
        {
            var path = _contentPathResolver.GetContentBasePath(criteria.ContentType, criteria.StoreId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);

            var searchResult = await storageProvider.SearchAsync(criteria.FolderUrl, criteria.Keyword);

            var result = searchResult.Results.OfType<BlobFolder>()
                .Select(x => x.ToContentModel())
                .OfType<ContentItem>()
                .Concat(searchResult.Results.OfType<BlobInfo>().Select(x => x.ToContentModel()))
                // question: here we exclude files that have "blogs" in name. It's wrong!
                .Where(x => criteria.FolderUrl != null || !x.Name.EqualsInvariant(ContentConstants.ContentTypes.Blogs))
                .ToArray();

            return result;
        }

        public async Task<IList<ContentFile>> EnumerateFiles(FilterFilesCriteria criteria)
        {
            var path = _contentPathResolver.GetContentBasePath(criteria.ContentType, criteria.StoreId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);

            var result = await EnumerateItemsRecursively(storageProvider, criteria.FolderUrl);
            return result;
        }

        private async Task<IList<ContentFile>> EnumerateItemsRecursively(IBlobContentStorageProvider storageProvider, string path)
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
