
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.CustomerModule.Data.Search.Indexing;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class PagesSearchRequestBuilder : ISearchRequestBuilder
    {
        private readonly ISearchPhraseParser _searchPhraseParser;

        public PagesSearchRequestBuilder(ISearchPhraseParser searchPhraseParser)
        {
            _searchPhraseParser = searchPhraseParser;
        }

        public virtual string DocumentType { get; } = ContentKnownDocumentTypes.MarkdownPages;

        public virtual SearchRequest BuildRequest(SearchCriteriaBase criteria)
        {
            SearchRequest request = null;

            var contentSearchCriteria = criteria as ContentSearchCriteria;
            if (contentSearchCriteria != null)
            {
                // Getting filters modifies search phrase
                var filters = GetFilters(contentSearchCriteria);

                request = new SearchRequest
                {
                    SearchKeywords = contentSearchCriteria.SearchPhrase,
                    SearchFields = new[] { IndexDocumentExtensions.SearchableFieldName },
                    Filter = filters.And(),
                    Sorting = GetSorting(contentSearchCriteria),
                    Skip = criteria.Skip,
                    Take = criteria.Take,
                };
            }

            return request;
        }


        protected virtual IList<IFilter> GetFilters(ContentSearchCriteria criteria)
        {
            var result = new List<IFilter>();

            if (!string.IsNullOrEmpty(criteria.SearchPhrase))
            {
                var parseResult = _searchPhraseParser.Parse(criteria.SearchPhrase);
                criteria.SearchPhrase = parseResult.SearchPhrase;
                result.AddRange(parseResult.Filters);
            }

            if (criteria.ObjectIds?.Any() == true)
            {
                result.Add(new IdsFilter { Values = criteria.ObjectIds });
            }

            //if (!string.IsNullOrEmpty(criteria.ContentType))
            //{
            //    result.Add(CreateTermFilter("relativeUrl", $"{ContentTypeUtility.GetContentBasePath(criteria.ContentType, null)}/"));
            //}

            //if (!string.IsNullOrEmpty(criteria.StoreId))
            //{
            //    result.Add(CreateTermFilter("relativeUrl", $"/{criteria.StoreId}/"));
            //}

            if (!criteria.FolderUrls.IsNullOrEmpty())
            {
                result.Add(CreateTermFilter("relativeUrl", criteria.FolderUrls));
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

        protected static IFilter CreateTermFilter(string fieldName, IEnumerable<string> values)
        {
            return new TermFilter
            {
                FieldName = fieldName,
                Values = values.ToArray(),
            };
        }
    }
}
