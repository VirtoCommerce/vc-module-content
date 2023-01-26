using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IContentFileService
    {
        Task<IList<ContentItem>> FilterFilesAsync(FilterFilesCriteria criteria);
        Task<IList<ContentFile>> EnumerateFiles(FilterFilesCriteria criteria);
    }
}
