using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.ContentModule.Web.Controllers.Api;
using VirtoCommerce.StoreModule.Core.Services;
using Xunit;

namespace VirtoCommerce.ContentModule.Tests;

/// <summary>
/// DELETE /api/content/{type}/{store}?urls=… is asked for a state — "these are gone" — and has to be
/// safe to ask twice. It used to take any url that was not a file for a folder and hand it to the
/// storage provider, which fails on a folder that is not there; a CI unpublish of a page already
/// removed by hand answered 500, and that single failure froze a deployment pipeline for a week.
/// </summary>
public class ContentControllerDeleteTests
{
    private const string ContentType = "pages";
    private const string StoreId = "vccom";

    private readonly Mock<IContentService> _contentService = new();
    private readonly Mock<IContentFileService> _contentFileService = new();

    public ContentControllerDeleteTests()
    {
        _contentService.Setup(x => x.ItemExistsAsync(ContentType, StoreId, It.IsAny<string>())).ReturnsAsync(false);
        _contentFileService.Setup(x => x.FilterItemsAsync(It.IsAny<FilterItemsCriteria>())).ReturnsAsync(new List<ContentItem>());
    }

    [Fact]
    public async Task DeleteContent_APageThatIsAlreadyGone_IsNoContent_AndDeletesNothing()
    {
        var result = await Controller().DeleteContent(ContentType, StoreId, ["blogs/integrations/old.page"]);

        Assert.IsType<NoContentResult>(result);
        _contentService.Verify(x => x.DeleteContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
    }

    [Fact]
    public async Task DeleteContent_AFile_DeletesItsDraftAndItsPublishedCopy()
    {
        _contentService.Setup(x => x.ItemExistsAsync(ContentType, StoreId, "blogs/news/post.page")).ReturnsAsync(true);
        _contentService.Setup(x => x.ItemExistsAsync(ContentType, StoreId, "blogs/news/post.page-draft")).ReturnsAsync(true);

        await Controller().DeleteContent(ContentType, StoreId, ["blogs/news/post.page"]);

        _contentService.Verify(x => x.DeleteContentAsync(ContentType, StoreId,
            It.Is<string[]>(urls => urls.Length == 2 && urls[0] == "blogs/news/post.page-draft" && urls[1] == "blogs/news/post.page")), Times.Once);
        // a file is a file; its parent is never listed to ask whether it is a folder
        _contentFileService.Verify(x => x.FilterItemsAsync(It.IsAny<FilterItemsCriteria>()), Times.Never);
    }

    [Fact]
    public async Task DeleteContent_AFolderItsParentLists_IsDeleted()
    {
        _contentFileService
            .Setup(x => x.FilterItemsAsync(It.Is<FilterItemsCriteria>(c => c.FolderUrl == "blogs" && c.ContentType == ContentType && c.StoreId == StoreId)))
            .ReturnsAsync(new List<ContentItem> { new ContentFolder { Name = "Archive" }, new ContentFile { Name = "index.page" } });

        await Controller().DeleteContent(ContentType, StoreId, ["blogs/archive"]);

        _contentService.Verify(x => x.DeleteContentAsync(ContentType, StoreId,
            It.Is<string[]>(urls => urls.Length == 1 && urls[0] == "blogs/archive")), Times.Once);
    }

    [Fact]
    public async Task DeleteContent_ARootFolder_IsLookedUpInTheRootListing_NotTheAdminsRootView()
    {
        // an empty folder url rather than null: null is the admin's root view, which hides "blogs"
        _contentFileService
            .Setup(x => x.FilterItemsAsync(It.Is<FilterItemsCriteria>(c => c.FolderUrl == string.Empty)))
            .ReturnsAsync(new List<ContentItem> { new ContentFolder { Name = "blogs" } });

        await Controller().DeleteContent(ContentType, StoreId, ["blogs"]);

        _contentService.Verify(x => x.DeleteContentAsync(ContentType, StoreId,
            It.Is<string[]>(urls => urls.Length == 1 && urls[0] == "blogs")), Times.Once);
    }

    [Fact]
    public async Task DeleteContent_AMixedList_DeletesOnlyWhatIsThere()
    {
        _contentService.Setup(x => x.ItemExistsAsync(ContentType, StoreId, "a.page")).ReturnsAsync(true);

        await Controller().DeleteContent(ContentType, StoreId, ["a.page", "gone.page", "gone-folder"]);

        _contentService.Verify(x => x.DeleteContentAsync(ContentType, StoreId,
            It.Is<string[]>(urls => urls.Length == 1 && urls[0] == "a.page")), Times.Once);
    }

    [Fact]
    public async Task DeleteContent_WhenTheParentCannotBeListed_TreatsTheFolderAsAbsent()
    {
        _contentFileService
            .Setup(x => x.FilterItemsAsync(It.IsAny<FilterItemsCriteria>()))
            .ThrowsAsync(new System.IO.DirectoryNotFoundException("no such directory"));

        var result = await Controller().DeleteContent(ContentType, StoreId, ["no/such/folder"]);

        Assert.IsType<NoContentResult>(result);
        _contentService.Verify(x => x.DeleteContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
    }

    private ContentController Controller() => new(
        Mock.Of<IContentStatisticService>(),
        _contentService.Object,
        _contentFileService.Object,
        Mock.Of<IFullTextContentSearchService>(),
        // the real thing: draft and published urls are its one job, and the test is about how they are used
        new PublishingServices(_contentService.Object),
        Mock.Of<IStoreService>(),
        NullLogger<ContentController>.Instance,
        new ConfigurationBuilder().Build());
}
