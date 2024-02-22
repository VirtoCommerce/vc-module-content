using System.IO;
using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Extensions;

namespace VirtoCommerce.ContentModule.FileSystem
{
    public class FileSystemContentBlobStorageProviderFactory : IBlobContentStorageProviderFactory
    {
        private readonly FileSystemContentBlobOptions _options;
        private readonly IFileExtensionService _fileExtensionService;
        private readonly IEventPublisher _eventPublisher;


        public FileSystemContentBlobStorageProviderFactory(IOptions<FileSystemContentBlobOptions> options, IFileExtensionService fileExtensionService, IEventPublisher eventPublisher)
        {
            _options = options.Value;
            _fileExtensionService = fileExtensionService;
            _eventPublisher = eventPublisher;
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
            return new FileSystemContentBlobStorageProvider(new OptionsWrapper<FileSystemContentBlobOptions>(clonedOptions), _fileExtensionService, _eventPublisher);
        }
    }
}
