
using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.ContentModule.Data.Utility;
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

        public virtual string DocumentType { get; } = ContentKnownDocumentTypes.Pages;

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
            var relativeUrl = new Uri(ContentTypeUtility.GetContentBasePath(criteria.ContentType, criteria.StoreId));

            if (!string.IsNullOrEmpty(criteria.FolderUrl))
            {
                relativeUrl = new Uri(relativeUrl, criteria.FolderUrl);
            }
            result.Add(CreateTermFilter("relativeUrl", relativeUrl.ToString()));

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
