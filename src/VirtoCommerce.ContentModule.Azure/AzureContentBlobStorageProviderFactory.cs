using System.IO;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Services;

namespace VirtoCommerce.ContentModule.Azure
{
    public class AzureContentBlobStorageProviderFactory : IBlobContentStorageProviderFactory
    {
        private readonly AzureContentBlobOptions _options;
        public AzureContentBlobStorageProviderFactory(IOptions<AzureContentBlobOptions> options)
        {
            _options = options.Value;
        }
        public IBlobContentStorageProvider CreateProvider(string basePath)
        {
            basePath = basePath?.Replace('/', Path.DirectorySeparatorChar)
                           .Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", $"{Path.DirectorySeparatorChar}");

            var clonedOptions = _options.Clone() as AzureContentBlobOptions;
            clonedOptions.RootPath = Path.Combine(clonedOptions.RootPath, basePath);
            return new AzureContentBlobStorageProvider(new OptionsWrapper<AzureContentBlobOptions>(clonedOptions));
        }
    }
}
