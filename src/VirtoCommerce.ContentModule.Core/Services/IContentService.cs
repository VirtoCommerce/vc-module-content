using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IContentService
    {
        Task DeleteContentAsync(string contentType, string storeId, string[] urls);
        Task MoveContentAsync(string contentType, string storeId, string oldPath, string newPath);
        Task CopyContentAsync(string contentType, string storeId, string srcPath, string destPath);
        Task CreateFolderAsync(string contentType, string storeId, ContentFolder folder);

        Task<bool> ItemExistsAsync(string contentType, string storeId, string relativeUrl);
        Task<Stream> GetItemStreamAsync(string contentType, string storeId, string relativeUrl);

        Task UnpackAsync(string contentType, string storeId, string archivePath, string destPath);

        Task<ContentFile> DownloadContentAsync(string contentType, string storeId, string srcUrl, string folderPath);
        Task<ContentFile> SaveContentAsync(string contentType, string storeId, string folderPath, string fileName, Stream content);
    }
}
