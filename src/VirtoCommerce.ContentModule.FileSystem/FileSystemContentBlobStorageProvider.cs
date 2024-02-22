using Microsoft.Extensions.Options;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.FileSystemAssetsModule.Core;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.ContentModule.FileSystem
{
    public class FileSystemContentBlobStorageProvider : FileSystemBlobProvider, IBlobContentStorageProvider
    {
        public FileSystemContentBlobStorageProvider(IOptions<FileSystemBlobOptions> options, IFileExtensionService fileExtensionService, IEventPublisher eventPublisher)
            : base(options, fileExtensionService, eventPublisher)
        {
        }
    }
}
