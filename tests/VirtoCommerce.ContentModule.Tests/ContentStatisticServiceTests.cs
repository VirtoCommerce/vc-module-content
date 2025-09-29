using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Services;
using Xunit;

namespace VirtoCommerce.ContentModule.Tests;

public class ContentStatisticServiceTests
{
    private const string StoreId = "TestStore";

    [Theory]
    [InlineData("file1.md", "file2.md", 2)]
    [InlineData("file1.md-draft", "file2.md-draft", 2)]
    [InlineData("file1.md-draft", "file2.md", 2)]
    [InlineData("file1.md-draft", "file1.md", 1)]
    public async Task GetStorePagesCount_IsCorrect(string filename1, string filename2, int expectedCount)
    {
        var sut = GetService([filename1, filename2]);
        var result = await sut.GetStorePagesCountAsync(StoreId);
        Assert.Equal(expectedCount, result);
    }

    private ContentStatisticService GetService(params string[] files)
    {
        var blobContentProviderFactory = new BlobContentStorageProviderStub(files);
        var contentPathResolver = new ContentPathResolverStub();

        var contentItemPathRegistrar = new Mock<IContentItemTypeRegistrar>();
        contentItemPathRegistrar.Setup(x => x.IsRegisteredContentItemType(It.IsAny<string>())).Returns(true);

        var contentService = new Mock<IContentService>();
        var publishingService = new PublishingServices(contentService.Object);
        var result = new ContentStatisticService(blobContentProviderFactory, contentPathResolver, contentItemPathRegistrar.Object, publishingService);
        return result;
    }

    private class ContentPathResolverStub : IContentPathResolver
    {
        public string GetContentBasePath(string contentType, string storeId, string themeName = null)
        {
            return string.Empty;
        }
    }

    private class BlobContentStorageProviderStub(string[] files) : IBlobContentStorageProviderFactory
    {
        public IBlobContentStorageProvider CreateProvider(string basePath)
        {
            return new ContentBlobStorageProviderStub(files.ToList());
        }
    }

    private class ContentBlobStorageProviderStub(List<string> files) : IBlobContentStorageProvider
    {
        public Task DeleteAsync(string blobUrl)
        {
            files.Remove(blobUrl);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string blobUrl)
        {
            var exists = files.Contains(blobUrl);
            return Task.FromResult(exists);
        }

        public Task<BlobEntrySearchResult> SearchAsync(string folderUrl, string keyword)
        {
            var result = new BlobEntrySearchResult
            {
                Results = files.Select(x => new BlobInfo { Name = x, RelativeUrl = x }).Cast<BlobEntry>().ToList(),
                TotalCount = files.Count
            };
            return Task.FromResult(result);
        }

        public Task<BlobInfo> GetBlobInfoAsync(string blobUrl)
        {
            var result = new BlobInfo();
            return Task.FromResult(result);
        }

        public Task CreateFolderAsync(BlobFolder folder)
        {
            return Task.CompletedTask;
        }

        public Stream OpenRead(string blobUrl)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> OpenReadAsync(string blobUrl)
        {
            throw new NotImplementedException();
        }

        public Stream OpenWrite(string blobUrl)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> OpenWriteAsync(string blobUrl)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string[] urls)
        {
            throw new NotImplementedException();
        }

        public void Move(string srcUrl, string destUrl)
        {
            throw new NotImplementedException();
        }

        public Task MoveAsyncPublic(string srcUrl, string destUrl)
        {
            throw new NotImplementedException();
        }

        public void Copy(string srcUrl, string destUrl)
        {
            throw new NotImplementedException();
        }

        public Task CopyAsync(string srcUrl, string destUrl)
        {
            throw new NotImplementedException();
        }

        public Task UploadAsync(string blobUrl, Stream content, string contentType, bool overwrite = true)
        {
            throw new NotImplementedException();
        }

        public string GetAbsoluteUrl(string blobKey)
        {
            throw new NotImplementedException();
        }
    }
}
