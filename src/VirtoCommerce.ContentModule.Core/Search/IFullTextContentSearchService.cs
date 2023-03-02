using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Search
{
    public interface IFullTextContentSearchService
    {
        Task<ContentSearchResult> SearchContentAsync(ContentSearchCriteria criteria);
    }
}
