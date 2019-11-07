using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentIndexedSearchService : IContentSearchService
    {
        private readonly ISearchRequestBuilder[] _searchRequestBuilders;
        private readonly ISearchProvider _searchProvider;
        private readonly IContentBlobStorageProvider _storageProvider;


        public ContentIndexedSearchService(ISearchRequestBuilder[] searchRequestBuilders, ISearchProvider searchProvider, Func<string, IContentBlobStorageProvider> contentStorageProviderFactory)
        {
            _searchRequestBuilders = searchRequestBuilders;
            _searchProvider = searchProvider;
            _storageProvider = contentStorageProviderFactory(string.Empty);
        }

        public virtual async Task<GenericSearchResult<BlobInfo>> SearchAsync(ContentSearchCriteria criteria)
        {
            var requestBuilder = GetRequestBuilder(criteria);
            var request = requestBuilder?.BuildRequest(criteria);

            var response = await _searchProvider.SearchAsync(criteria.ObjectType, request);

            var result = ConvertResponse(response, criteria);
            return result;
        }


        protected virtual ISearchRequestBuilder GetRequestBuilder(ContentSearchCriteria criteria)
        {
            var requestBuilder = _searchRequestBuilders?.FirstOrDefault(b => b.DocumentType.Equals(criteria.ObjectType)) ??
                                 _searchRequestBuilders?.FirstOrDefault(b => string.IsNullOrEmpty(b.DocumentType));

            if (requestBuilder == null)
                throw new InvalidOperationException($"No query builders found for document type '{criteria.ObjectType}'");

            return requestBuilder;
        }

        protected virtual GenericSearchResult<BlobInfo> ConvertResponse(SearchResponse response, ContentSearchCriteria criteria)
        {
            var result = AbstractTypeFactory<GenericSearchResult<BlobInfo>>.TryCreateInstance();

            if (response != null)
            {
                result.TotalCount = (int)response.TotalCount;
                result.Results = ConvertDocuments(response.Documents, criteria);
            }

            return result;
        }

        protected virtual ICollection<BlobInfo> ConvertDocuments(IList<SearchDocument> documents, ContentSearchCriteria criteria)
        {
            var result = new List<BlobInfo>();
            if (documents?.Any() == true)
            {
                var itemIds = documents.Select(doc => doc.Id).ToArray();
                var items = itemIds.Select(x => _storageProvider.GetBlobInfo(x))
                    .ToList();

                result.AddRange(items);
            }

            return result;
        }
    }
}
