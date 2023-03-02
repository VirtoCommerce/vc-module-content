using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.AzureBlobAssetsModule.Core;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core;
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

        public override async Task<BlobEntrySearchResult> SearchAsync(string folderUrl, string keyword)
        {
            var result = await base.SearchAsync(folderUrl, keyword);
            return result;
        }
    }
}
