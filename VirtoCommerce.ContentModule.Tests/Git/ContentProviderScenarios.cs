using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Data.Services;
using Xunit;
using Xunit.Abstractions;

namespace VirtoCommerce.ContentModule.Tests.Git
{
    public class ContentProviderScenarios : IDisposable
    {
        private static string _tempDir;
        private readonly ITestOutputHelper _output;

        public ContentProviderScenarios(ITestOutputHelper output)
        {
            _output = output;
            _tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDir);
        }

        [Fact]
        public void Can_create_file_in_master()
        {
            var fileName = Path.Combine("master", "page.json");
            var storageProvider = GetProvider();

            using (var blobStream = storageProvider.OpenWrite(fileName))
            {
            }

            Assert.True(File.Exists(Path.Combine(_tempDir, fileName)));
        }


        [Fact]
        public void Can_create_file_in_draft()
        {
            // TODO: create file in a different branch/environment, also create a new environment from master
            var fileName = Path.Combine("draft", "page1.json");
            var storageProvider = GetProvider();

            using (var blobStream = storageProvider.OpenWrite(fileName))
            {
            }

            Assert.True(File.Exists(Path.Combine(_tempDir, fileName)));
        }

        [Fact]
        public void Can_publish_file_from_draft_to_master()
        {
            // TODO: create file in a different branch/environment, also create a new environment from master
        }

        private FileSystemContentBlobStorageProvider GetProvider()
        {
            var storageProvider = new FileSystemContentBlobStorageProvider(_tempDir, "http://localhost");
            return storageProvider;
        }

        public void Dispose()
        {
            Directory.Delete(_tempDir, true);
        }
    }
}
