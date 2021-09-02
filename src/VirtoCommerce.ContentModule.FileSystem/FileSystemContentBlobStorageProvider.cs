using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Assets.FileSystem;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ContentModule.FileSystem
{
    public class FileSystemContentBlobStorageProvider : FileSystemBlobProvider, IBlobContentStorageProvider
    {
        public FileSystemContentBlobStorageProvider(IOptions<FileSystemContentBlobOptions> options, IOptions<PlatformOptions> platformOptions, ISettingsManager settingsManager)
            : base(options, platformOptions, settingsManager)
        {
        }
    }
}
