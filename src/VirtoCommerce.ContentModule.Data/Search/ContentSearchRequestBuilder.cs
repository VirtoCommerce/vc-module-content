using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extenstions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentSearchRequestBuilder : ISearchRequestBuilder
    {
        public virtual string DocumentType => FullTextContentSearchService.ContentDocumentType;

        public virtual Task<SearchRequest> BuildRequestAsync(SearchCriteriaBase criteria)
        {
            SearchRequest request = null;

            var searchCriteria = criteria as ContentSearchCriteria;
            if (searchCriteria != null)
            {
                // Getting filters modifies search phrase
                //var filters = GetFilters(searchCriteria);

                request = new SearchRequest
                {
                    SearchKeywords = searchCriteria.Keyword,
                    SearchFields = new[] { IndexDocumentExtensions.SearchableFieldName },
                    Filter = new TermFilter
                    {
                        FieldName = "StoreId",
                        Values = new[] { searchCriteria.StoreId }
                    },
                    //Sorting = GetSorting(searchCriteria),
                    Skip = criteria.Skip,
                    Take = criteria.Take,
                };
            }

            return Task.FromResult(request);
        }
    }
}
