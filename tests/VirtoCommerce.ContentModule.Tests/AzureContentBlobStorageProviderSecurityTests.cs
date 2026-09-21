using System;
using Microsoft.Extensions.Options;
using Moq;
using VirtoCommerce.AssetsModule.Core.Services;
using VirtoCommerce.ContentModule.Azure;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Exceptions;
using Xunit;

namespace VirtoCommerce.ContentModule.Tests
{
    /// <summary>
    /// VCST-6016 (Defect 4): NormalizeUrl must not let a request-supplied absolute URL
    /// escape the configured content root (container) and reach a foreign container.
    /// </summary>
    [Trait("Category", "Unit")]
    public class AzureContentBlobStorageProviderSecurityTests
    {
        private const string ConfiguredContainer = "cms";

        [Fact]
        public void NormalizeUrl_WithForeignAbsoluteUrl_IsRejected()
        {
            var provider = BuildProvider(ConfiguredContainer);

            // An absolute URL whose first path segment names a DIFFERENT container must not
            // be honoured - otherwise a request parameter selects the storage container.
            Assert.Throws<PlatformException>(() =>
                provider.NormalizeUrl("http://attacker.invalid/othercontainer/vault/"));
        }

        [Fact]
        public void NormalizeUrl_WithAbsoluteUrlInsideConfiguredRoot_IsAllowed()
        {
            var provider = BuildProvider(ConfiguredContainer);

            // A legitimate absolute URL that points inside the configured container is preserved
            // (e.g. the sitemap module passes full URLs).
            var result = provider.NormalizeUrl("https://acc.blob.core.windows.net/cms/pages/index.html");

            Assert.StartsWith("/cms", result, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void NormalizeUrl_WithRelativeUrl_StaysWithinConfiguredRoot()
        {
            var provider = BuildProvider(ConfiguredContainer);

            var result = provider.NormalizeUrl("pages/index.html");

            Assert.Contains(ConfiguredContainer, result, StringComparison.OrdinalIgnoreCase);
        }

        private static AzureContentBlobStorageProvider BuildProvider(string rootPath)
        {
            var options = Options.Create(new AzureContentBlobOptions
            {
                // Parsed but never connected to - NormalizeUrl performs no network I/O.
                ConnectionString = "UseDevelopmentStorage=true",
                RootPath = rootPath,
            });

            var fileExtensionService = new Mock<IFileExtensionService>();
            fileExtensionService.Setup(s => s.IsExtensionAllowedAsync(It.IsAny<string>())).ReturnsAsync(true);

            return new AzureContentBlobStorageProvider(options, fileExtensionService.Object, Mock.Of<IEventPublisher>());
        }
    }
}
