using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.AzureBlobAssetsModule.Core;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Extensions;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ContentModule.Azure
{
    public class AzureContentBlobStorageProvider : AzureBlobProvider, IBlobContentStorageProvider
    {
        private readonly AzureContentBlobOptions _options;
        public AzureContentBlobStorageProvider(IOptions<AzureContentBlobOptions> options, IOptions<PlatformOptions> platformOptions, ISettingsManager settingsManager)
            : base(options, platformOptions, settingsManager)
        {
            _options = options.Value;
        }

        public override Stream OpenRead(string url)
        {
            return base.OpenRead(NormalizeUrl(url));
        }

        public override Task<BlobInfo> GetBlobInfoAsync(string url)
        {
            return base.GetBlobInfoAsync(NormalizeUrl(url));
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

        public override Stream OpenWrite(string blobUrl)
        {
            return base.OpenWrite(NormalizeUrl(blobUrl));
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

            var rootAzurePath = _options.RootPath.Replace('\\', '/').Trim('/').Length + 1;

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
                if (url.IsAbsoluteUrl())
                {
                    url = Uri.UnescapeDataString(new Uri(url).AbsolutePath);
                }
                //var cleanUrl = url.StartsWith(_options.RootPath) ? url[_options.RootPath.Length..] : url;
                result = UrlHelperExtensions.Combine(_options.RootPath, url);
            }
            return result;
        }
    }
}
