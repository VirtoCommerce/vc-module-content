using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentSearchRequestBuilder : ISearchRequestBuilder
    {
        private readonly ISearchPhraseParser _searchPhraseParser;

        public virtual string DocumentType => FullTextContentSearchService.ContentDocumentType;

        public ContentSearchRequestBuilder(ISearchPhraseParser searchPhraseParser)
        {
            _searchPhraseParser = searchPhraseParser;
        }

        public virtual Task<SearchRequest> BuildRequestAsync(SearchCriteriaBase criteria)
        {
            SearchRequest result = null;

            if (criteria is ContentSearchCriteria searchCriteria)
            {
                // GetFilters() modifies Keyword
                searchCriteria = searchCriteria.CloneTyped();
                var filters = GetFilters(searchCriteria);

                result = new SearchRequest
                {
                    SearchKeywords = searchCriteria.Keyword,
                    SearchFields = new[] { IndexDocumentExtensions.ContentFieldName },
                    Filter = filters.And(),
                    Skip = criteria.Skip,
                    Take = criteria.Take,
                };
            }

            return Task.FromResult(result);
        }

        protected virtual IList<IFilter> GetFilters(ContentSearchCriteria criteria)
        {
            var result = new List<IFilter>();

            if (!string.IsNullOrEmpty(criteria.Keyword))
            {
                var parseResult = _searchPhraseParser.Parse(criteria.Keyword);
                criteria.Keyword = parseResult.Keyword;
                result.AddRange(parseResult.Filters);
            }

            if (criteria.ObjectIds?.Any() == true)
            {
                result.Add(new IdsFilter { Values = criteria.ObjectIds });
            }

            if (!string.IsNullOrEmpty(criteria.StoreId))
            {
                result.Add(CreateTermFilter("StoreId", criteria.StoreId));
            }

            if (!string.IsNullOrEmpty(criteria.LanguageCode))
            {
                result.Add(
                    CreateTermFilter("CultureName", criteria.LanguageCode)
                        .Or(CreateTermFilter("CultureName", "any")
                ));
            }

            if (!string.IsNullOrEmpty(criteria.CultureName))
            {
                result.Add(CreateTermFilter("CultureName", criteria.CultureName));
            }
            if (!string.IsNullOrEmpty(criteria.FolderUrl))
            {
                result.Add(CreateTermFilter("FolderUrl", criteria.FolderUrl));
            }
            if (!string.IsNullOrEmpty(criteria.ContentType))
            {
                result.Add(CreateTermFilter("ContentType", criteria.ContentType));
            }


            return result;
        }

        protected virtual IList<SortingField> GetSorting(ContentSearchCriteria criteria)
        {
            var result = new List<SortingField>();

            foreach (var sortInfo in criteria.SortInfos)
            {
                var fieldName = sortInfo.SortColumn.ToLowerInvariant();
                var isDescending = sortInfo.SortDirection == SortDirection.Descending;
                result.Add(new SortingField(fieldName, isDescending));
            }

            return result;
        }

        protected static IFilter CreateTermFilter(string fieldName, string value)
        {
            return new TermFilter
            {
                FieldName = fieldName,
                Values = new[] { value },
            };
        }
    }
}
