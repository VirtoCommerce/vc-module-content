using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Assets.AzureBlobStorage;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
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

        public override Stream OpenWrite(string url)
        {
            return base.OpenWrite(NormalizeUrl(url));
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

            var rootAzurePath = _options.RootPath.Replace('\\', '/');

            foreach (var blobEntry in result.Results)
            {
                blobEntry.RelativeUrl = blobEntry.RelativeUrl.Replace($"/{rootAzurePath}", string.Empty);
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
                result = Path.DirectorySeparatorChar + url.Replace('/', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
                result = _options.RootPath + Path.DirectorySeparatorChar + result.Replace(_options.RootPath, string.Empty);
                //TODO: need to use Path.DirectorySeparatorChar instead of hardcoded value
                result = Regex.Replace(result, @"\\+", $"{Path.DirectorySeparatorChar}");
            }
            return result;
        }
    }
}
