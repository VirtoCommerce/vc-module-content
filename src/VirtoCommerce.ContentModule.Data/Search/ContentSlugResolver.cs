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
            var criteria = new ContentSearchCriteria
            {
                Keyword = ""
            };
            var searchResults = await _searchService.SearchContentAsync(criteria);
            var result = searchResults.Results.Select(x => new SeoInfo
            {
                Name = x.Name,
                //SemanticUrl = x.,
                //PageTitle =,
                //MetaDescription =,
                //ImageAltDescription =,
                //MetaKeywords =,
                StoreId = x.StoreId,
                //ObjectId = x.,
                ObjectType = FullTextContentSearchService.ContentDocumentType,
                //IsActive =,

            });
            return result.ToArray();
        }
    }
}
