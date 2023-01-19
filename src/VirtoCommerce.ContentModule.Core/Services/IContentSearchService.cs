using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IContentSearchService
    {
        Task<ContentItem[]> FilterContentAsync(string contentType, string storeId, string folderUrl, string keyword);
        Task<IEnumerable<ContentFile>> EnumerateItems(string contentType, string storeId, string folderUrl);
    }
}
