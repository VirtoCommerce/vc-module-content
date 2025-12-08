using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Data.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search;

public class ContentSearchRequestBuilder(ISearchPhraseParser searchPhraseParser, IStoreService storeService)
    : ISearchRequestBuilder
{
    public virtual string DocumentType => FullTextContentSearchService.ContentDocumentType;

    public virtual async Task<SearchRequest> BuildRequestAsync(SearchCriteriaBase criteria)
    {
        SearchRequest result = null;

        if (criteria is ContentSearchCriteria searchCriteria)
        {
            // GetFilters() modifies Keyword
            searchCriteria = searchCriteria.CloneTyped();
            var filters = await GetFilters(searchCriteria);

            result = new SearchRequest
            {
                SearchKeywords = searchCriteria.Keyword,
                SearchFields = new[] { IndexDocumentExtensions.ContentFieldName },
                Filter = filters.And(),
                Sorting = GetSorting(searchCriteria),
                Skip = criteria.Skip,
                Take = criteria.Take,
            };
        }

        return result;
    }

    protected virtual async Task<IList<IFilter>> GetFilters(ContentSearchCriteria criteria)
    {
        var result = new List<IFilter>();

        if (!string.IsNullOrEmpty(criteria.Keyword))
        {
            var parseResult = searchPhraseParser.Parse(criteria.Keyword);
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

        await AddLanguageFilter(criteria, result);
        AddDateFilter(criteria, result);
        AddUserGroups(criteria, result);
        AddOrganizationFilter(criteria, result);

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

    private static void AddUserGroups(ContentSearchCriteria criteria, List<IFilter> result)
    {
        if (criteria.UserGroups == null)
        {
            return;
        }
        var userGroups = criteria.UserGroups;
        var filter = new TermFilter
        {
            FieldName = "UserGroups",
            Values =
            [
                ContentItemConverter.Any,
                    ..userGroups
            ]
        };
        result.Add(filter);
    }

    private static void AddDateFilter(ContentSearchCriteria criteria, List<IFilter> result)
    {
        if (criteria.ActiveOn.HasValue)
        {
            var date = criteria.ActiveOn.Value;
            var dateFilter = CreateDateFilter("StartDate", date, true)
                .And(CreateDateFilter("EndDate", date, false));
            result.Add(dateFilter);
        }
    }

    private static void AddOrganizationFilter(ContentSearchCriteria criteria, List<IFilter> result)
    {
        if (criteria.OrganizationId.IsNullOrEmpty())
        {
            return;
        }
        var anyFilter = new TermFilter
        {
            FieldName = "OrganizationId",
            Values = [ContentItemConverter.Any, criteria.OrganizationId],
        };
        result.Add(anyFilter);
    }

    private async Task AddLanguageFilter(ContentSearchCriteria criteria, List<IFilter> filter)
    {
        var cultureFilter = CreateTermFilter("CultureName", ContentItemConverter.Any);
        var useFilter = false;

        if (!criteria.CultureName.IsNullOrEmpty())
        {
            useFilter = true;
            cultureFilter = cultureFilter.Or(CreateTermFilter("CultureName", criteria.CultureName));
        }
        if (!criteria.LanguageCode.IsNullOrEmpty())
        {
            useFilter = true;
            cultureFilter = cultureFilter.Or(CreateTermFilter("CultureName", criteria.LanguageCode));
        }

        if (useFilter)
        {
            var store = await storeService.GetByIdAsync(criteria.StoreId);
            var storeLanguage = store.DefaultLanguage;
            if (!storeLanguage.IsNullOrEmpty())
            {
                cultureFilter = cultureFilter.Or(CreateTermFilter("CultureName", storeLanguage));
            }
            filter.Add(cultureFilter);
        }
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
            Values = [value],
        };
    }

    protected static IFilter CreateDateFilter(string fieldName, DateTime date, bool isLess)
    {
        return new RangeFilter
        {
            FieldName = fieldName,
            Values =
            [
                new RangeFilterValue
                    {
                        IncludeLower = true,
                        IncludeUpper = true,
                        Lower = isLess ? null : date.ToString("O"),
                        Upper = isLess ? date.ToString("O") : null
                    }
            ]
        };
    }
}
