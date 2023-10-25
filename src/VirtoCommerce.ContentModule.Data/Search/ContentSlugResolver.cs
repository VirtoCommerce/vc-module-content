using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.CoreModule.Core.Seo;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentSlugResolver : ISeoBySlugResolver
    {
        private readonly IFullTextContentSearchService _searchService;

        public ContentSlugResolver(IFullTextContentSearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<SeoInfo[]> FindSeoBySlugAsync(string slug)
        {
            var list = await FindWithSlash(slug);
            var other = await FindWithoutSlash(slug);
            var result = list.Union(other).DistinctBy(x => x.ObjectId);
            return result.ToArray();
        }

        private async Task<IEnumerable<SeoInfo>> FindWithSlash(string sourceSlug)
        {
            if (!sourceSlug.StartsWith("/"))
            {
                sourceSlug = "/" + sourceSlug;
            }

            return await FindInternal(sourceSlug);
        }

        private async Task<IEnumerable<SeoInfo>> FindWithoutSlash(string sourceSlug)
        {
            if (sourceSlug.StartsWith("/"))
            {
                sourceSlug = sourceSlug.Substring(1);
            }

            return await FindInternal(sourceSlug);
        }

        private async Task<IEnumerable<SeoInfo>> FindInternal(string slug)
        {
            var criteria = new ContentSearchCriteria
            {
                Keyword = "permalink:" + slug,
                Skip = 0,
                Take = 100
            };

            var result = new List<SeoInfo>();

            while (true)
            {
                var searchResults = await _searchService.SearchContentAsync(criteria);

                if (searchResults.Results.Count == 0)
                {
                    break;
                }

                var items = searchResults.Results.Select(x => new SeoInfo
                {
                    Name = x.Name,
                    SemanticUrl = x.Permalink,
                    StoreId = x.StoreId,
                    ObjectId = x.Id,
                    ObjectType = FullTextContentSearchService.ContentDocumentType
                });

                result.AddRange(items);
                criteria.Skip += criteria.Take;
            }

            return result;
        }
    }
}
