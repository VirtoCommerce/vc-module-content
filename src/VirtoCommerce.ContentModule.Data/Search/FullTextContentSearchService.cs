using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.SearchModule.Data.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class FullTextContentSearchService : IFullTextContentSearchService
    {
        public const string ContentDocumentType = nameof(ContentFile);

        private readonly ISearchRequestBuilderRegistrar _searchRequestBuilderRegistrar;
        private readonly ISearchProvider _searchProvider;
        private readonly IContentService _contentService;

        public FullTextContentSearchService(ISearchRequestBuilderRegistrar searchRequestBuilderRegistrar, ISearchProvider searchProvider, IContentService contentService)
        {
            _searchRequestBuilderRegistrar = searchRequestBuilderRegistrar;
            _searchProvider = searchProvider;
            _contentService = contentService;
        }

        public async Task<ContentSearchResult> SearchContentAsync(ContentSearchCriteria criteria)
        {
            var requestBuilder = GetRequestBuilder(criteria);
            var request = await requestBuilder?.BuildRequestAsync(criteria);

            var response = await _searchProvider.SearchAsync(criteria.ObjectType, request);

            var result = await ConvertResponseAsync(response, criteria);
            return result;
        }

        protected virtual ISearchRequestBuilder GetRequestBuilder(ContentSearchCriteria criteria)
        {
            return _searchRequestBuilderRegistrar.GetRequestBuilderByDocumentType(criteria.ObjectType);
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
                var itemPaths = documents.Select(doc => doc.Id).ToArray();
                var items = await GetItemsByPathsAsync(itemPaths, criteria);
                return items;
                //var itemsMap = items.ToDictionary(m => m.Name, m => m);

                //// Preserve documents order
                //var filteredItems = documents
                //    .Select(doc => itemsMap.ContainsKey(doc.Id) ? itemsMap[doc.Id] : null)
                //    .Where(m => m != null)
                //.ToArray();
                //result.AddRange(filteredItems);
            }
            return new List<IndexableContentFile>();
        }

        protected virtual async Task<IList<IndexableContentFile>> GetItemsByPathsAsync(IList<string> itemPaths, ContentSearchCriteria criteria)
        {
            var result = new List<IndexableContentFile>();
            foreach (var item in itemPaths)
            {
                var contentItem = await _contentService.GetFileContentAsync(ContentConstants.ContentTypes.Pages, criteria.StoreId, item);
                if (contentItem != null)
                {
                    contentItem.StoreId = criteria.StoreId;
                    result.Add(contentItem);
                }
            }
            return result;
        }
    }
}
