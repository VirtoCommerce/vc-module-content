using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.ContentModule.Core.Extensions;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.CoreModule.Core.Seo;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Search;

public class ContentSeoResolver : ISeoResolver
{
    private readonly IFullTextContentSearchService _searchService;
    private readonly IConfiguration _configuration;

    public ContentSeoResolver(IFullTextContentSearchService searchService, IConfiguration configuration)
    {
        _searchService = searchService;
        _configuration = configuration;
    }

    public async Task<IList<SeoInfo>> FindSeoAsync(SeoSearchCriteria criteria)
    {
        if (!_configuration.IsContentFullTextSearchEnabled())
        {
            return Array.Empty<SeoInfo>();
        }

        var result = (await FindInternal(criteria.Permalink)).DistinctBy(x => x.ObjectId);
        return result.ToArray();
    }

    private async Task<IList<SeoInfo>> FindInternal(string permalink)
    {
        if (!permalink.StartsWith('/'))
        {
            permalink = "/" + permalink;
        }
        var criteria = new ContentSearchCriteria
        {
            Keyword = "permalink:" + permalink,
            Skip = 0,
            Take = 100,
        };

        var searchResults = await _searchService.SearchAllNoCloneAsync(criteria);
        return searchResults.Select(x => new SeoInfo
        {
            Name = x.DisplayName,
            SemanticUrl = x.Permalink,
            StoreId = x.StoreId,
            LanguageCode = x.Language,
            ObjectId = x.Id,
            Id = x.Id,
            IsActive = true,
            ObjectType = FullTextContentSearchService.ContentDocumentType,
        }).ToList();
    }
}
