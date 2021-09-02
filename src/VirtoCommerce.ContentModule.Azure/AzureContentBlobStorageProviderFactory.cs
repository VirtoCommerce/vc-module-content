using System.IO;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.ContentModule.Azure
{
    public class AzureContentBlobStorageProviderFactory : IBlobContentStorageProviderFactory
    {
        private readonly AzureContentBlobOptions _options;
        private readonly IOptions<PlatformOptions> _platformOptions;
        private readonly ISettingsManager _settingsManager;
        public AzureContentBlobStorageProviderFactory(IOptions<AzureContentBlobOptions> options, IOptions<PlatformOptions> platformOptions, ISettingsManager settingsManager)
        {
            _options = options.Value;
            _platformOptions = platformOptions;
            _settingsManager = settingsManager;
        }
        public IBlobContentStorageProvider CreateProvider(string basePath)
        {
            basePath = basePath?.Replace('/', Path.DirectorySeparatorChar)
                           .Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}");

            var clonedOptions = _options.Clone() as AzureContentBlobOptions;
            clonedOptions.RootPath = Path.Combine(clonedOptions.RootPath, basePath);
            return new AzureContentBlobStorageProvider(new OptionsWrapper<AzureContentBlobOptions>(clonedOptions), _platformOptions, _settingsManager);
        }
    }
}
