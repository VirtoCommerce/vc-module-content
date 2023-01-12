using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services.Indexing
{
    public interface IFullTextContentSearchService
    {
        Task<ContentSearchResult> SearchContentAsync(ContentSearchCriteria criteria);
    }
}
