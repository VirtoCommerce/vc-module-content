using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.ContentModule.Core.Extensions;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Exceptions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class FullTextContentSearchService : IFullTextContentSearchService
    {
        public const string ContentDocumentType = nameof(ContentFile);

        private readonly ISearchRequestBuilderRegistrar _searchRequestBuilderRegistrar;
        private readonly ISearchProvider _searchProvider;
        private readonly IContentService _contentService;
        private readonly IConfiguration _configuration;

        public FullTextContentSearchService(
            ISearchRequestBuilderRegistrar searchRequestBuilderRegistrar,
            ISearchProvider searchProvider,
            IContentService contentService,
            IConfiguration configuration)
        {
            _searchRequestBuilderRegistrar = searchRequestBuilderRegistrar;
            _searchProvider = searchProvider;
            _contentService = contentService;
            _configuration = configuration;
        }

        public async Task<ContentSearchResult> SearchContentAsync(ContentSearchCriteria criteria)
        {
            if (!_configuration.IsContentFullTextSearchEnabled())
            {
                throw new SearchException("Indexed content search is disabled. To enable it add 'Search:ContentFullTextSearchEnabled' configuration key to app settings and set it to true.");
            }

            var requestBuilder = GetRequestBuilder(criteria);

            if (requestBuilder == null)
            {
                return null;
            }

            var request = await requestBuilder.BuildRequestAsync(criteria);
            var response = await _searchProvider.SearchAsync(ContentDocumentType, request);
            var result = await ConvertResponseAsync(response, criteria);

            return result;
        }

        protected virtual ISearchRequestBuilder GetRequestBuilder(ContentSearchCriteria criteria)
        {
            return _searchRequestBuilderRegistrar.GetRequestBuilderByDocumentType(ContentDocumentType);
        }

        protected virtual async Task<ContentSearchResult> ConvertResponseAsync(SearchResponse response, ContentSearchCriteria criteria)
        {
            var result = AbstractTypeFactory<ContentSearchResult>.TryCreateInstance();

            if (response != null)
            {
                result.TotalCount = (int)response.TotalCount;
                result.Results = await ConvertDocumentsAsync(response.Documents, criteria);
            }

            return result;
        }

        protected virtual async Task<IList<IndexableContentFile>> ConvertDocumentsAsync(IList<SearchDocument> documents, ContentSearchCriteria criteria)
        {
            if (documents?.Any() == true)
            {
                var items = await GetItemsByPathsAsync(documents, criteria);
                return items;
            }

            return new List<IndexableContentFile>();
        }

        protected virtual async Task<IList<IndexableContentFile>> GetItemsByPathsAsync(IList<SearchDocument> documents, ContentSearchCriteria criteria)
        {
            var result = new List<IndexableContentFile>();

            foreach (var document in documents)
            {
                var (storeId, contentType, relativeUrl) = DocumentIdentifierHelper.ParseId(document.Id);
                var contentItem = await _contentService.GetFileContentAsync(contentType, storeId, relativeUrl);
                if (contentItem != null)
                {
                    result.Add(contentItem);
                }
            }

            return result;
        }
    }
}
