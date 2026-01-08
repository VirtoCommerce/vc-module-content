using System.IO;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IContentService
    {
        Task DeleteContentAsync(string contentType, string storeId, string[] urls);
        Task MoveContentAsync(string contentType, string storeId, string oldPath, string newPath);
        Task CopyContentAsync(string contentType, string storeId, string srcPath, string destPath);
        Task<ContentFile> GetFileAsync(string contentType, string storeId, string relativeUrl);
        Task<IndexableContentFile> GetFileContentAsync(string contentType, string storeId, string relativeUrl);

        /// <summary>
        /// Asynchronously retrieves the content of a file identified by the specified ID.
        /// </summary>
        /// <param name="id">The unique identifier of the file to retrieve. Cannot be null or empty. It is used as documentId for fulltext search service.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see
        /// cref="IndexableContentFile"/> representing the file content if found; otherwise, <see langword="null"/>.</returns>
        Task<IndexableContentFile> GetFileContentAsync(string id);
        Task CreateFolderAsync(string contentType, string storeId, ContentFolder folder);

        Task<bool> ItemExistsAsync(string contentType, string storeId, string relativeUrl);
        Task<Stream> GetItemStreamAsync(string contentType, string storeId, string relativeUrl);

        /// <summary>
        /// Asynchronously retrieves a stream for reading the content of the item with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the item to retrieve. Cannot be null or empty. It is used as documentId for fulltext search service also.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a stream for reading the item's
        /// content.</returns>
        Task<Stream> GetItemStreamAsync(string id);

        Task UnpackAsync(string contentType, string storeId, string archivePath, string destPath);

        Task<ContentFile> DownloadContentAsync(string contentType, string storeId, string srcUrl, string folderPath);
        Task<ContentFile> SaveContentAsync(string contentType, string storeId, string folderPath, string fileName, Stream content);
        Task CopyFileAsync(string contentType, string storeId, string srcFile, string destFile);
    }
}
