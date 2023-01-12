using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Model.Search;
using VirtoCommerce.ContentModule.Core.Services;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.ContentModule.Data.Extensions;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class ContentSearchServices : IContentSearchService
    {
        private readonly IBlobContentStorageProviderFactory _blobContentStorageProviderFactory;
        private readonly IContentPathResolver _contentPathResolver;

        public ContentSearchServices(
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
    }
}
