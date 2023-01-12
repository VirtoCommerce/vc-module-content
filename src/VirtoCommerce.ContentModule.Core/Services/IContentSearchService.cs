using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Model.Search;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IContentSearchService
    {
        Task<ContentItem[]> FilterContentAsync(string contentType, string storeId, string folderUrl, string keyword);
    }
}
