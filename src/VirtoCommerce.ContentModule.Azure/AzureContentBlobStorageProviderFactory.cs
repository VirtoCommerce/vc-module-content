using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Extensions;

namespace VirtoCommerce.ContentModule.Azure
{
    public class AzureContentBlobStorageProviderFactory : IBlobContentStorageProviderFactory
    {
        private readonly AzureContentBlobOptions _options;
        private readonly IFileExtensionService _fileExtensionService;

        public AzureContentBlobStorageProviderFactory(IOptions<AzureContentBlobOptions> options, IFileExtensionService fileExtensionService)
        {
            _options = options.Value;
            _fileExtensionService = fileExtensionService;
        }

        public IBlobContentStorageProvider CreateProvider(string basePath)
        {
            var clonedOptions = _options.CloneTyped();
            clonedOptions.RootPath = UrlHelperExtensions.Combine(clonedOptions.RootPath, basePath);
            return new AzureContentBlobStorageProvider(Options.Create(clonedOptions), _fileExtensionService);
        }
    }
}
