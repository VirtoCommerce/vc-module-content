using System.IO;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Extensions;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ContentModule.FileSystem
{
    public class FileSystemContentBlobStorageProviderFactory : IBlobContentStorageProviderFactory
    {
        private readonly FileSystemContentBlobOptions _options;
        private readonly IOptions<PlatformOptions> _platformOptions;
        private readonly ISettingsManager _settingsManager;


        public FileSystemContentBlobStorageProviderFactory(IOptions<FileSystemContentBlobOptions> options, IOptions<PlatformOptions> platformOptions, ISettingsManager settingsManager)
        {
            _options = options.Value;
            _platformOptions = platformOptions;
            _settingsManager = settingsManager;
        }

        public IBlobContentStorageProvider CreateProvider(string basePath)
        {
            var clonedOptions = _options.Clone() as FileSystemContentBlobOptions;

            var storagePath = Path.Combine(clonedOptions.RootPath, basePath.Replace('/', Path.DirectorySeparatorChar));
            //Use content api/content as public url by default             
            var publicPath = $"~/api/content/{basePath}?relativeUrl=";
            if (!string.IsNullOrEmpty(clonedOptions.PublicUrl))
            {
                publicPath = UrlHelperExtensions.Combine(clonedOptions.PublicUrl, basePath);
            }
            clonedOptions.RootPath = storagePath;
            clonedOptions.PublicUrl = publicPath;
            //Do not export default theme (Themes/default) its will distributed with code
            return new FileSystemContentBlobStorageProvider(new OptionsWrapper<FileSystemContentBlobOptions>(clonedOptions), _platformOptions, _settingsManager);
        }
    }
}
