using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Extensions;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Caching;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.Helpers;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

using UrlHelperExtensions = VirtoCommerce.Platform.Core.Extensions.UrlHelperExtensions;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class ContentService : IContentService
    {
        private readonly IBlobContentStorageProviderFactory _blobContentStorageProviderFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly ICrudService<Store> _storeService;
        private readonly IContentPathResolver _contentPathResolver;
        private readonly IHttpClientFactory _httpClientFactory;

        public ContentService(
                IBlobContentStorageProviderFactory blobContentStorageProviderFactory,
                IPlatformMemoryCache platformMemoryCache,
                IStoreService storeService,
                IContentPathResolver contentPathResolver,
                IHttpClientFactory httpClientFactory
            )
        {
            _blobContentStorageProviderFactory = blobContentStorageProviderFactory;
            _platformMemoryCache = platformMemoryCache;
            _storeService = (ICrudService<Store>)storeService;
            _contentPathResolver = contentPathResolver;
            _httpClientFactory = httpClientFactory;
        }

        public async Task DeleteContentAsync(string contentType, string storeId, string[] urls)
        {
            var path = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);
            await storageProvider.RemoveAsync(urls);

            //ToDo Reset cached items
            //_cacheManager.ClearRegion($"content-{storeId}");
            ContentCacheRegion.ExpireRegion();
        }

        public async Task MoveContentAsync(string contentType, string storeId, string oldPath, string newPath)
        {
            var path = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);
            await storageProvider.MoveAsyncPublic(oldPath, newPath);
        }

        public async Task CopyContentAsync(string contentType, string storeId, string srcPath, string destPath)
        {
            // question: here should be absolute urls only?
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(string.Empty);

            // note: This method used only for default themes copying that we use string.Empty instead storeId because default themes placed only in root content folder
            await storageProvider.CopyAsync(srcPath, destPath);
        }

        public async Task<ContentFile> GetFileAsync(string contentType, string storeId, string relativeUrl)
        {
            var path = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);
            var blobInfo = await storageProvider.GetBlobInfoAsync(relativeUrl);

            var result = blobInfo.ToContentModel();
            return result;
        }

        public async Task<IndexableContentFile> GetFileContentAsync(string contentType, string storeId, string relativeUrl)
        {
            var path = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);
            var blobInfo = await storageProvider.GetBlobInfoAsync(relativeUrl);

            var result = blobInfo.ToIndexableContentModel();
            var fileStream = await storageProvider.OpenReadAsync(relativeUrl);
            using var reader = new StreamReader(fileStream);
            result.Content = await reader.ReadToEndAsync();
            return result;
        }

        public async Task CreateFolderAsync(string contentType, string storeId, ContentFolder folder)
        {
            var path = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);
            await storageProvider.CreateFolderAsync(folder.ToBlobModel(AbstractTypeFactory<BlobFolder>.TryCreateInstance()));
        }

        public async Task<bool> ItemExistsAsync(string contentType, string storeId, string relativeUrl)
        {
            var path = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);
            var blobInfo = await storageProvider.GetBlobInfoAsync(relativeUrl);
            return blobInfo != null;
        }

        public async Task<Stream> GetItemStreamAsync(string contentType, string storeId, string relativeUrl)
        {
            var path = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);
            var fileStream = await storageProvider.OpenReadAsync(relativeUrl);
            return fileStream;
        }

        public async Task UnpackAsync(string contentType, string storeId, string archivePath, string destPath)
        {
            var path = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(path);

            using (var stream = storageProvider.OpenRead(archivePath))
            using (var archive = new ZipArchive(stream))
            {
                // count number of root folders, if one, we use our standard approach of ignoring root folder
                var foldersCount = archive.Entries.Where(x => x.FullName.Split('/').Length > 1 || x.FullName.EndsWith("/"))
                    .Select(f => f.FullName.Split('/')[0])
                    .Distinct()
                    .Count();

                foreach (var entry in archive.Entries)
                    if (!entry.FullName.EndsWith("/"))
                    {
                        var fileName = foldersCount == 1 ? string.Join("/", entry.FullName.Split('/').Skip(1)) : entry.FullName;

                        using (var entryStream = entry.Open())
                        using (var targetStream = storageProvider.OpenWrite(destPath + "/" + fileName))
                        {
                            entryStream.CopyTo(targetStream);
                        }
                    }
            }

            //remove archive after unpack
            await storageProvider.RemoveAsync(new[] { archivePath });
        }

        public async Task<ContentFile> DownloadContentAsync(string contentType, string storeId, string srcUrl, string folderPath)
        {
            var fileName = HttpUtility.UrlDecode(Path.GetFileName(srcUrl));
            var fileUrl = UrlHelperExtensions.Combine(folderPath ?? "", fileName);

            var targetPath = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(targetPath);

            using var client = _httpClientFactory.CreateClient();
            using var blobStream = storageProvider.OpenWrite(fileUrl);
            using var remoteStream = await client.GetStreamAsync(srcUrl);

            remoteStream.CopyTo(blobStream);

            var сontentFile = AbstractTypeFactory<ContentFile>.TryCreateInstance();

            сontentFile.Name = fileName;
            сontentFile.Url = storageProvider.GetAbsoluteUrl(fileUrl);
            return сontentFile;
        }

        public async Task<ContentFile> SaveContentAsync(string contentType, string storeId, string folderPath, string fileName, Stream content)
        {
            var targetPath = _contentPathResolver.GetContentBasePath(contentType, storeId);
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(targetPath);

            var targetFilePath = UrlHelperExtensions.Combine(folderPath ?? "", fileName);

            using (var targetStream = storageProvider.OpenWrite(targetFilePath))
            {
                await content.CopyToAsync(targetStream);
            }

            var contentFile = AbstractTypeFactory<ContentFile>.TryCreateInstance();
            contentFile.Name = fileName;
            contentFile.Url = storageProvider.GetAbsoluteUrl(targetFilePath);
            return contentFile;
        }
    }
}
