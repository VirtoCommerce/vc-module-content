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


        public ContentIndexedSearchService(ISearchRequestBuilder[] searchRequestBuilders, ISearchProvider searchProvider)
        {
            _searchRequestBuilders = searchRequestBuilders;
            _searchProvider = searchProvider;

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

            //if (documents?.Any() == true)
            //{
            //    var itemIds = documents.Select(doc => doc.Id).ToArray();
            //    var items = GeMembersByIds(itemIds, criteria);
            //    var itemsMap = items.ToDictionary(m => m.Id, m => m);

            //    // Preserve documents order
            //    var members = documents
            //        .Select(doc => itemsMap.ContainsKey(doc.Id) ? itemsMap[doc.Id] : null)
            //        .Where(m => m != null)
            //        .ToArray();

            //    result.AddRange(members);
            //}

            return result;
        }
    }
}
