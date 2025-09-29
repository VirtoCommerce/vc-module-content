using System.Linq;
using System.Threading.Tasks;
using Moq;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Services;
using Xunit;

namespace VirtoCommerce.ContentModule.Tests;

public class PublishingServiceTests
{
    private const string ContentType = "content";
    private const string StoreId = "storeId";

    [Theory]
    [InlineData("file.md", false)]
    [InlineData("file.md-draft", true)]
    [InlineData("file", false)]
    public void IsDraft_IsCorrect(string filename, bool expectedIsDraft)
    {
        var contentService = new Mock<IContentService>();
        var sut = new PublishingServices(contentService.Object);

        var result = sut.IsDraft(filename);
        Assert.Equal(expectedIsDraft, result);
    }

    [Theory]
    [InlineData("file.md", true, "file.md-draft")]
    [InlineData("file.md-draft", true, "file.md-draft")]
    [InlineData("file.md", false, "file.md")]
    [InlineData("file.md-draft", false, "file.md")]
    public void GetRelativeDraftUrlTests(string source, bool isDraft, string expectedTarget)
    {
        var contentService = new Mock<IContentService>();
        var sut = new PublishingServices(contentService.Object);

        var result = sut.GetRelativeDraftUrl(source, isDraft);
        Assert.Equal(expectedTarget, result);
    }

    #region PublishingAsync

    [Fact]
    public async Task PublishingAsync_MakeDraft()
    {
        var (sut, contentService) = GetPublishingService([("file.md", true), ("file.md-draft", false)]);

        await sut.PublishingAsync(ContentType, StoreId, "file.md", publish: false);

        contentService.Verify(x => x.MoveContentAsync(ContentType, StoreId, "file.md", "file.md-draft"), Times.Exactly(1));
        contentService.Verify(x => x.DeleteContentAsync(ContentType, StoreId, It.IsAny<string[]>()), Times.Never);
    }

    [Fact]
    public async Task PublishingAsync_PublishFile()
    {
        var (sut, contentService) = GetPublishingService([("file.md", false), ("file.md-draft", true)]);

        await sut.PublishingAsync(ContentType, StoreId, "file.md", publish: true);

        contentService.Verify(x => x.MoveContentAsync(ContentType, StoreId, "file.md-draft", "file.md"), Times.Exactly(1));
        contentService.Verify(x => x.DeleteContentAsync(ContentType, StoreId, It.IsAny<string[]>()), Times.Never);
    }

    [Fact]
    public async Task PublishingAsync_PublishWhenPublishedExists()
    {
        var (sut, contentService) = GetPublishingService([("file.md", true), ("file.md-draft", true)]);

        await sut.PublishingAsync(ContentType, StoreId, "file.md", publish: true);

        contentService.Verify(x => x.MoveContentAsync(ContentType, StoreId, "file.md-draft", "file.md"), Times.Exactly(1));

        string[] filesToRemove = ["file.md"];
        contentService.Verify(x => x.DeleteContentAsync(ContentType, StoreId, filesToRemove), Times.Exactly(1));
    }

    [Fact]
    public async Task PublishingAsync_DontUnpublishWithDraft()
    {
        var (sut, contentService) = GetPublishingService([("file.md", true), ("file.md-draft", true)]);

        await sut.PublishingAsync(ContentType, StoreId, "file.md", publish: false);

        contentService.Verify(x => x.MoveContentAsync(ContentType, StoreId, It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        contentService.Verify(x => x.DeleteContentAsync(ContentType, StoreId, It.IsAny<string[]>()), Times.Never);
    }

    #endregion

    #region PublishStatusAsync

    [Fact]
    public async Task PublishStatusAsync_Published()
    {
        var (sut, _) = GetPublishingService([("file.md", true), ("file.md-draft", false)]);

        var result = await sut.PublishStatusAsync(ContentType, StoreId, "file.md");
        Assert.True(result.Published);
        Assert.False(result.HasChanges);
    }

    [Fact]
    public async Task PublishStatusAsync_Unpublished()
    {
        var (sut, _) = GetPublishingService([("file.md", false), ("file.md-draft", true)]);

        var result = await sut.PublishStatusAsync(ContentType, StoreId, "file.md");
        Assert.False(result.Published);
        Assert.True(result.HasChanges);
    }

    [Fact]
    public async Task PublishStatusAsync_HasChanged()
    {
        var (sut, _) = GetPublishingService([("file.md", true), ("file.md-draft", true)]);

        var result = await sut.PublishStatusAsync(ContentType, StoreId, "file.md");
        Assert.True(result.Published);
        Assert.True(result.HasChanges);
    }

    #endregion

    [Theory]
    [InlineData(1, 0, 0, 1, "file.md")]
    [InlineData(0, 1, 1, 1, "file.md-draft")]
    [InlineData(1, 0, 1, 1, "file.md-draft", "file.md")]
    [InlineData(1, 1, 1, 2, "file1.md-draft", "file2.md")]
    [InlineData(2, 0, 1, 2, "file1.md-draft", "file1.md", "file2.md")]
    [InlineData(1, 1, 2, 2, "file1.md-draft", "file1.md", "file2.md-draft")]
    public async Task SetFileStatuses(int expectedPublishedCount, int expectedUnpublishedCount, int expectedHasChangesCount, int expectedTotalCount, params string[] filenames)
    {
        var contentService = new Mock<IContentService>();
        var sut = new PublishingServices(contentService.Object);

        ContentFile CreateContentFile(string filename)
        {
            return new ContentFile
            {
                Name = filename,
                RelativeUrl = filename,
            };
        }

        var files = filenames.Select(CreateContentFile);

        var result = (await sut.SetFilesStatuses(files)).ToList();

        Assert.Equal(expectedPublishedCount, result.Count(x => x.Published));
        Assert.Equal(expectedUnpublishedCount, result.Count(x => !x.Published));
        Assert.Equal(expectedHasChangesCount, result.Count(x => x.HasChanges));
        Assert.Equal(expectedTotalCount, result.Count());
    }

    private (PublishingServices, Mock<IContentService>) GetPublishingService((string Filename, bool Exists)[] files)
    {
        var contentService = new Mock<IContentService>();

        foreach (var file in files)
        {
            contentService
                .Setup(x => x.ItemExistsAsync(ContentType, StoreId, file.Filename))
                .ReturnsAsync(file.Exists);
        }

        return (new PublishingServices(contentService.Object), contentService);
    }
}
