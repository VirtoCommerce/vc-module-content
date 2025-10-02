using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using VirtoCommerce.ContentModule.Core.Extensions;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Seo.Core.Models;
using VirtoCommerce.Seo.Core.Services;

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
            return [];
        }

        return (await FindFiles(criteria.Permalink ?? criteria.Slug))
            .DistinctBy(x => x.Id)
            .Select(x => new SeoInfo
            {
                Name = x.DisplayName,
                SemanticUrl = x.Permalink,
                StoreId = x.StoreId,
                LanguageCode = x.Language,
                ObjectId = x.Id,
                Id = x.Id,
                IsActive = true,
                ObjectType = FullTextContentSearchService.ContentDocumentType
            })
            .OrderBy(x => x.LanguageCode)
            .ToList();
    }

    private Task<IList<IndexableContentFile>> FindFiles(string permalink)
    {
        if (!permalink.StartsWith('/'))
        {
            permalink = "/" + permalink;
        }

        var criteria = new ContentSearchCriteria
        {
            Keyword = $"permalink:\"{permalink}\"",
            Take = 100,
        };

        return _searchService.SearchAllNoCloneAsync(criteria);
    }
}
