using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services;

public interface IPublishingService
{
    Task PublishingAsync(string contentType, string storeId, string relativeUrl, bool publish);
    Task<IEnumerable<ContentFile>> SetFilesStatuses(IEnumerable<ContentFile> files);
    string GetRelativeDraftUrl(string source, bool draft);
    Task<FilePublishStatus> PublishStatusAsync(string contentType, string storeId, string relativeUrl);
}
