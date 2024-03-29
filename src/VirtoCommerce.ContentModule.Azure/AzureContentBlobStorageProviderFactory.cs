using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Extensions;

namespace VirtoCommerce.ContentModule.Azure
{
    public class AzureContentBlobStorageProviderFactory : IBlobContentStorageProviderFactory
    {
        private readonly AzureContentBlobOptions _options;
        private readonly IFileExtensionService _fileExtensionService;
        private readonly IEventPublisher _eventPublisher;

        public AzureContentBlobStorageProviderFactory(IOptions<AzureContentBlobOptions> options, IFileExtensionService fileExtensionService, IEventPublisher eventPublisher)
        {
            _options = options.Value;
            _fileExtensionService = fileExtensionService;
            _eventPublisher = eventPublisher;
        }
        public IBlobContentStorageProvider CreateProvider(string basePath)
        {
            var clonedOptions = _options.Clone() as AzureContentBlobOptions;
            clonedOptions.RootPath = UrlHelperExtensions.Combine(clonedOptions.RootPath, basePath);
            return new AzureContentBlobStorageProvider(new OptionsWrapper<AzureContentBlobOptions>(clonedOptions), _fileExtensionService, _eventPublisher);
        }
    }
}
