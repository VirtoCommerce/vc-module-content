using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IContentFileService
    {
        Task<IList<ContentItem>> FilterItemsAsync(FilterItemsCriteria criteria);
        Task<IList<ContentFile>> EnumerateFiles(FilterItemsCriteria criteria);
    }
}
