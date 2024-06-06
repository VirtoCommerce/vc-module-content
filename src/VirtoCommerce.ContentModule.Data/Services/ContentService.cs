using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core.Events;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Extensions;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using UrlHelperExtensions = VirtoCommerce.Platform.Core.Extensions.UrlHelperExtensions;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class ContentService : IContentService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IBlobContentStorageProviderFactory _blobContentStorageProviderFactory;
        private readonly IContentPathResolver _contentPathResolver;
        private readonly IHttpClientFactory _httpClientFactory;

        public ContentService(
            IBlobContentStorageProviderFactory blobContentStorageProviderFactory,
            IContentPathResolver contentPathResolver,
            IEventPublisher eventPublisher,
            IHttpClientFactory httpClientFactory)
        {
            _eventPublisher = eventPublisher;
            _blobContentStorageProviderFactory = blobContentStorageProviderFactory;
            _contentPathResolver = contentPathResolver;
            _httpClientFactory = httpClientFactory;
        }

        public async Task DeleteContentAsync(string contentType, string storeId, string[] urls)
        {
            var storageProvider = GetStorageProvider(contentType, storeId);
            await storageProvider.RemoveAsync(urls);

            ContentCacheRegion.ExpireRegion();
            var changedEntries = urls.Select(x => ContentItemConverter.CreateChangedEntry(x, null, EntryState.Deleted));
            var changedEvent = new ContentFileChangedEvent(contentType, storeId, changedEntries);
            await _eventPublisher.Publish(changedEvent);
        }

        public async Task MoveContentAsync(string contentType, string storeId, string oldPath, string newPath)
        {
            var storageProvider = GetStorageProvider(contentType, storeId);
            await storageProvider.MoveAsyncPublic(oldPath, newPath);

            var changedEntry = ContentItemConverter.CreateChangedEntry(oldPath, newPath);
            var changedEvent = new ContentFileChangedEvent(contentType, storeId, [changedEntry]);
            await _eventPublisher.Publish(changedEvent);
        }

        public async Task CopyContentAsync(string contentType, string storeId, string srcPath, string destPath)
        {
            // question: here should be absolute urls only?
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(string.Empty);

            // note: This method used only for default themes copying that we use string.
            // Empty instead storeId because default themes placed only in root content folder
            await storageProvider.CopyAsync(srcPath, destPath);

            var changedEntry = ContentItemConverter.CreateChangedEntry(null, destPath);
            var changedEvent = new ContentFileChangedEvent(contentType, storeId, [changedEntry]);
            await _eventPublisher.Publish(changedEvent);
        }

        public async Task<ContentFile> GetFileAsync(string contentType, string storeId, string relativeUrl)
        {
            var storageProvider = GetStorageProvider(contentType, storeId);
            var blobInfo = await storageProvider.GetBlobInfoAsync(relativeUrl);
            var result = blobInfo.ToContentModel();

            return result;
        }

        public async Task<IndexableContentFile> GetFileContentAsync(string contentType, string storeId, string relativeUrl)
        {
            var storageProvider = GetStorageProvider(contentType, storeId);
            var blobInfo = await storageProvider.GetBlobInfoAsync(HttpUtility.UrlDecode(relativeUrl));

            if (blobInfo is null)
            {
                return null;
            }

            var result = blobInfo.ToIndexableContentModel();
            var fileStream = await storageProvider.OpenReadAsync(relativeUrl);
            using var reader = new StreamReader(fileStream);
            result.Content = await reader.ReadToEndAsync();
            result.StoreId = storeId;
            result.Language = relativeUrl.GetLanguage();
            result.ContentType = contentType;
            result.ParentUrl = result.RelativeUrl.GetParentUrl();
            return result;
        }

        public async Task CreateFolderAsync(string contentType, string storeId, ContentFolder folder)
        {
            var storageProvider = GetStorageProvider(contentType, storeId);
            await storageProvider.CreateFolderAsync(folder.ToBlobModel(AbstractTypeFactory<BlobFolder>.TryCreateInstance()));
        }

        public async Task<bool> ItemExistsAsync(string contentType, string storeId, string relativeUrl)
        {
            var storageProvider = GetStorageProvider(contentType, storeId);
            var blobInfo = await storageProvider.GetBlobInfoAsync(relativeUrl);
            return blobInfo != null;
        }

        public async Task<Stream> GetItemStreamAsync(string contentType, string storeId, string relativeUrl)
        {
            var storageProvider = GetStorageProvider(contentType, storeId);
            var blobInfo = await storageProvider.GetBlobInfoAsync(relativeUrl);
            var fileStream = await storageProvider.OpenReadAsync(blobInfo.RelativeUrl);
            return fileStream;
        }

        public async Task UnpackAsync(string contentType, string storeId, string archivePath, string destPath)
        {
            var storageProvider = GetStorageProvider(contentType, storeId);

            var changedEntries = new List<GenericChangedEntry<ContentFile>>();

            await using (var stream = await storageProvider.OpenReadAsync(archivePath))
            using (var archive = new ZipArchive(stream))
            {
                // count number of root folders, if one, we use our standard approach of ignoring root folder
                var foldersCount = archive.Entries.Where(x => x.FullName.Split('/').Length > 1 || x.FullName.EndsWith("/"))
                    .Select(f => f.FullName.Split('/')[0])
                    .Distinct()
                    .Count();

                foreach (var entry in archive.Entries)
                {
                    if (!entry.FullName.EndsWith("/"))
                    {
                        var fileName = foldersCount == 1 ? string.Join("/", entry.FullName.Split('/').Skip(1)) : entry.FullName;

                        var fullPath = Path.Combine(destPath, fileName);

                        await using var entryStream = entry.Open();
                        await using var targetStream = await storageProvider.OpenWriteAsync(fullPath);
                        await entryStream.CopyToAsync(targetStream);
                        changedEntries.Add(ContentItemConverter.CreateChangedEntry(null, fullPath));
                    }
                }
            }

            var changedEvent = new ContentFileChangedEvent(contentType, storeId, changedEntries);
            await _eventPublisher.Publish(changedEvent);

            //remove archive after unpack
            await storageProvider.RemoveAsync([archivePath]);
        }

        public async Task<ContentFile> DownloadContentAsync(string contentType, string storeId, string srcUrl, string folderPath)
        {
            var fileName = HttpUtility.UrlDecode(Path.GetFileName(srcUrl));

            using var client = _httpClientFactory.CreateClient();
            await using var content = await client.GetStreamAsync(srcUrl);

            var result = await SaveContentAsync(contentType, storeId, folderPath, fileName, content);
            return result;
        }

        public async Task<ContentFile> SaveContentAsync(string contentType, string storeId, string folderPath, string fileName, Stream content)
        {
            var storageProvider = GetStorageProvider(contentType, storeId);
            var targetFilePath = UrlHelperExtensions.Combine(folderPath ?? "", fileName);

            await using var targetStream = await storageProvider.OpenWriteAsync(targetFilePath);
            await content.CopyToAsync(targetStream);

            var result = await CreateContentFile(storageProvider, targetFilePath, folderPath);

            var changedEntry = ContentItemConverter.CreateChangedEntry(null, targetFilePath);
            var changedEvent = new ContentFileChangedEvent(contentType, storeId, [changedEntry]);
            await _eventPublisher.Publish(changedEvent);

            return result;
        }

        public async Task CopyFileAsync(string contentType, string storeId, string srcFile, string destFile)
        {
            var storageProvider = GetStorageProvider(contentType, storeId);
            await using var src = await storageProvider.OpenReadAsync(srcFile);
            await using var dest = await storageProvider.OpenWriteAsync(destFile);
            await src.CopyToAsync(dest);

            var changedEntry = ContentItemConverter.CreateChangedEntry(null, destFile);
            var changedEvent = new ContentFileChangedEvent(contentType, storeId, [changedEntry]);
            await _eventPublisher.Publish(changedEvent);
        }

        private IBlobContentStorageProvider GetStorageProvider(string contentType, string storeId)
        {
            var path = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);
            return storageProvider;
        }

        private static async Task<ContentFile> CreateContentFile(IBlobContentStorageProvider provider, string targetFilePath, string folderPath)
        {
            var blob = await provider.GetBlobInfoAsync(targetFilePath);
            var contentFile = AbstractTypeFactory<ContentFile>.TryCreateInstance();
            contentFile.Name = blob.Name;
            contentFile.RelativeUrl = blob.RelativeUrl;
            contentFile.ParentUrl = folderPath;
            contentFile.Url = blob.Url;
            return contentFile;
        }
    }
}
