using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.AzureBlobAssetsModule.Core;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Extensions;

namespace VirtoCommerce.ContentModule.Azure
{
    public class AzureContentBlobStorageProvider : AzureBlobProvider, IBlobContentStorageProvider
    {
        private readonly AzureContentBlobOptions _options;
        public AzureContentBlobStorageProvider(IOptions<AzureContentBlobOptions> options, IFileExtensionService fileExtensionService, IEventPublisher eventPublisher)
            : base(options, fileExtensionService, eventPublisher)
        {
            _options = options.Value;
        }

        public override async Task<BlobInfo> GetBlobInfoAsync(string blobUrl)
        {
            var result = await base.GetBlobInfoAsync(NormalizeUrl(blobUrl));
            if (result != null)
            {
                var rootAzurePath = _options.RootPath.Replace('\\', '/').Trim('/').Length + 1;
                result.RelativeUrl = result.RelativeUrl[rootAzurePath..];
            }
            return result;
        }

        public override Task CreateFolderAsync(BlobFolder folder)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));

            if (folder.ParentUrl.IsNullOrEmpty())
            {
                folder.Name = NormalizeUrl(folder.Name);
            }
            return base.CreateFolderAsync(folder);
        }

        public override Task<Stream> OpenWriteAsync(string blobUrl)
        {
            return base.OpenWriteAsync(NormalizeUrl(blobUrl));
        }

        public override Task<Stream> OpenReadAsync(string blobUrl)
        {
            return base.OpenReadAsync(NormalizeUrl(blobUrl));
        }

        public override Task RemoveAsync(string[] urls)
        {
            urls = urls.Select(NormalizeUrl).ToArray();

            return base.RemoveAsync(urls);
        }

        public override async Task<BlobEntrySearchResult> SearchAsync(string folderUrl, string keyword)
        {
            folderUrl = NormalizeUrl(folderUrl);

            var result = await base.SearchAsync(folderUrl, keyword);

            var rootAzurePath = _options.RootPath.Trim('/').Length + 1;

            foreach (var blobEntry in result.Results)
            {
                blobEntry.RelativeUrl = blobEntry.RelativeUrl[rootAzurePath..];
            }

            return result;
        }

        /// <summary>
        /// Chroot url (artificial add parent 'chroot' folder)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string NormalizeUrl(string url)
        {
            var result = _options.RootPath;
            if (!string.IsNullOrEmpty(url))
            {
                // url must not be absolute here (but sometime it is so, f.i. in sitemap module)
                // we need another function to get blobs by full url
                if (url.IsAbsoluteUrl())
                {
                    result = Uri.UnescapeDataString(new Uri(url).AbsolutePath);
                }
                else
                {
                    result = UrlHelperExtensions.Combine(_options.RootPath, url);
                }
            }
            return result;
        }
    }
}
