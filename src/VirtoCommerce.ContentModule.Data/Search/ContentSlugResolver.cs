using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.CoreModule.Core.Seo;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentSlugResolver : ISeoBySlugResolver
    {
        private readonly ContentSeoResolver _seoResolver;

        public ContentSlugResolver(ContentSeoResolver seoResolver)
        {
            _seoResolver = seoResolver;
        }

        public async Task<SeoInfo[]> FindSeoBySlugAsync(string slug)
        {
            var result = await _seoResolver.FindSeoAsync(new SeoSearchCriteria { Permalink = slug });
            return result.ToArray();
        }
    }
}
