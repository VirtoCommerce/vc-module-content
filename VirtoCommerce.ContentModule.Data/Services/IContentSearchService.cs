using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Data.Search;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Assets;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public interface IContentSearchService
    {
        Task<GenericSearchResult<BlobInfo>> SearchAsync(ContentSearchCriteria criteria);
    }
}
