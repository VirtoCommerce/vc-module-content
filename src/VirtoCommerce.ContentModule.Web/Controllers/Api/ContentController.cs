using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Extensions;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.ContentModule.Web.Filters;
using VirtoCommerce.ContentModule.Web.Validators;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Data.Helpers;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using Permissions = VirtoCommerce.ContentModule.Core.ContentConstants.Security.Permissions;

namespace VirtoCommerce.ContentModule.Web.Controllers.Api;

[Route("api/content/{contentType}/{storeId}")]
public class ContentController(
            IContentStatisticService contentStats,
            IContentService contentService,
            IContentFileService contentFileService,
            IFullTextContentSearchService fullTextContentSearchService,
            IPublishingService publishingService,
            IStoreService storeService,
            ILogger<ContentController> logger,
            IConfiguration configuration)
        : Controller
{
    private readonly IPublishingService _publishingService = publishingService;
    private static readonly FormOptions _defaultFormOptions = new();

    /// <summary>
    ///     Return summary content statistic
    /// </summary>
    /// <param name="storeId"></param>
    /// <returns>Object contains counters with main content types</returns>
    [HttpGet]
    [Route("~/api/content/{storeId}/stats")]
    [Authorize(Permissions.Read)]
    public async Task<ActionResult<ContentStatistic>> GetStoreContentStats(string storeId)
    {
        var pagesTask = contentStats.GetStorePagesCountAsync(storeId);
        var blogsTask = contentStats.GetStoreBlogsCountAsync(storeId);
        var themesTask = contentStats.GetStoreThemesCountAsync(storeId);

        var storeTask = storeService.GetNoCloneAsync(storeId, StoreResponseGroup.DynamicProperties.ToString());

        await Task.WhenAll(themesTask, blogsTask, pagesTask, storeTask);

        var activeThemeProperty = storeTask.Result.DynamicProperties.FirstOrDefault(x => x.Name == "DefaultThemeName");
        var activeTheme = activeThemeProperty?.Values?.FirstOrDefault()?.Value?.ToString();

        var result = new ContentStatistic
        {
            PagesCount = pagesTask.Result,
            BlogsCount = blogsTask.Result,
            ThemesCount = themesTask.Result,

            ActiveThemeName = activeTheme ?? ContentConstants.DefaultTheme
        };
        return Ok(result);
    }

    /// <summary>
    ///     Delete content from server
    /// </summary>
    /// <param name="contentType">possible values Themes or Pages</param>
    /// <param name="storeId">Store id </param>
    /// <param name="urls">relative content urls to delete</param>
    /// <returns></returns>
    [HttpDelete]
    [Route("")]
    [Authorize(Permissions.Delete)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteContent(string contentType, string storeId, [FromQuery] string[] urls)
    {
        var urlsToRemove = new List<string>();
        foreach (var url in urls)
        {
            var draftUrl = _publishingService.GetRelativeDraftUrl(url, true);
            var publishedUrl = _publishingService.GetRelativeDraftUrl(url, false);
            if (await contentService.ItemExistsAsync(contentType, storeId, draftUrl))
            {
                urlsToRemove.Add(draftUrl);
            }
            if (await contentService.ItemExistsAsync(contentType, storeId, publishedUrl))
            {
                urlsToRemove.Add(publishedUrl);
            }
        }
        await contentService.DeleteContentAsync(contentType, storeId, urlsToRemove.ToArray());
        return NoContent();
    }

    /// <summary>
    ///     Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager)
    /// </summary>
    /// <param name="contentType">possible values Themes or Pages</param>
    /// <param name="storeId">Store id</param>
    /// <param name="relativeUrl">content relative url</param>
    /// <param name="draft">Whether th given file be read from draft if it is exist</param>
    /// <returns>stream</returns>
    [HttpGet]
    [Route("")]
    [Authorize(Permissions.Read)]
    public async Task<ActionResult<byte[]>> GetContentItemDataStream(string contentType, string storeId, [FromQuery] string relativeUrl, [FromQuery] bool draft = false)
    {
        if (draft)
        {
            // use the draft logic, try to load the draft file, and if it isn't found, load to published version
            var draftUrl = _publishingService.GetRelativeDraftUrl(relativeUrl, true);
            if (await contentService.ItemExistsAsync(contentType, storeId, draftUrl))
            {
                var result = await contentService.GetItemStreamAsync(contentType, storeId, draftUrl);
                return File(result, MimeTypeResolver.ResolveContentType(relativeUrl));
            }
            var sourceUrl = _publishingService.GetRelativeDraftUrl(relativeUrl, false);
            if (await contentService.ItemExistsAsync(contentType, storeId, sourceUrl))
            {
                var result = await contentService.GetItemStreamAsync(contentType, storeId, sourceUrl);
                return File(result, MimeTypeResolver.ResolveContentType(relativeUrl));
            }
        }
        else
        {
            // use default flow
            if (await contentService.ItemExistsAsync(contentType, storeId, relativeUrl))
            {
                var result = await contentService.GetItemStreamAsync(contentType, storeId, relativeUrl);
                return File(result, MimeTypeResolver.ResolveContentType(relativeUrl));
            }
        }
        return NotFound();
    }

    /// <summary>
    ///     Search content items in specified folder and using search keyword
    /// </summary>
    /// <param name="contentType">possible values Themes or Pages</param>
    /// <param name="storeId">Store id</param>
    /// <param name="folderUrl">relative path for folder where content items will be searched</param>
    /// <param name="keyword">search keyword</param>
    /// <returns>content items</returns>
    [HttpGet]
    [Route("search")]
    [Authorize(Permissions.Read)]
    public async Task<ActionResult<ContentItem[]>> SearchContent(string contentType, string storeId, [FromQuery] string folderUrl = null, [FromQuery] string keyword = null)
    {
        var criteria = AbstractTypeFactory<FilterItemsCriteria>.TryCreateInstance();
        criteria.ContentType = contentType;
        criteria.StoreId = storeId;
        criteria.FolderUrl = folderUrl;
        criteria.Keyword = keyword;
        var result = await contentFileService.FilterItemsAsync(criteria);
        var folders = result.Where(x => x is not ContentFile);
        var files = await _publishingService.SetFilesStatuses(result.OfType<ContentFile>());
        var response = folders.Union(files);
        return Ok(response);
    }

    /// <summary>
    ///     Fulltext content search
    /// </summary>
    /// <param name="criteria">Search criteria</param>
    /// <returns>content items</returns>
    [HttpPost]
    [Route("~/api/content/search")]
    [Authorize(Permissions.Read)]
    public async Task<ActionResult<ContentItem[]>> FulltextSearchContent([FromBody] ContentSearchCriteria criteria)
    {
        criteria.Skip = 0;
        criteria.Take = 100;
        var result = await fullTextContentSearchService.SearchAllAsync(criteria);
        var response = _publishingService.SetFilesStatuses(result);
        return Ok(response);
    }

    [HttpGet]
    [Route("~/api/content/search/enabled")]
    public ActionResult GetContentFullTextSearchEnabled()
    {
        var result = configuration.IsContentFullTextSearchEnabled();
        return Ok(new { Result = result });
    }

    /// <summary>
    ///     Rename or move content item
    /// </summary>
    /// <param name="contentType">possible values Themes or Pages</param>
    /// <param name="storeId">Store id</param>
    /// <param name="oldUrl">old content item relative or absolute url</param>
    /// <param name="newUrl">new content item relative or absolute url</param>
    /// <returns></returns>
    [HttpGet]
    [Route("move")]
    [Authorize(Permissions.Update)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> MoveContent(string contentType, string storeId, [FromQuery] string oldUrl, [FromQuery] string newUrl)
    {
        var publishedSrc = _publishingService.GetRelativeDraftUrl(oldUrl, false);
        var unpublishedSrc = _publishingService.GetRelativeDraftUrl(oldUrl, true);

        var isFile = false;

        if (await contentService.ItemExistsAsync(contentType, storeId, publishedSrc))
        {
            isFile = true;
            var publishedDest = _publishingService.GetRelativeDraftUrl(newUrl, false);
            await contentService.MoveContentAsync(contentType, storeId, publishedSrc, publishedDest);
        }

        if (await contentService.ItemExistsAsync(contentType, storeId, unpublishedSrc))
        {
            isFile = true;
            var unpublishedDest = _publishingService.GetRelativeDraftUrl(newUrl, true);
            await contentService.MoveContentAsync(contentType, storeId, unpublishedSrc, unpublishedDest);
        }

        if (!isFile)
        {
            // it's a folder
            await contentService.MoveContentAsync(contentType, storeId, oldUrl, newUrl);
        }
        return NoContent();
    }

    /// <summary>
    /// Copy folder contents
    /// </summary>
    /// <param name="srcPath">source content relative path</param>
    /// <param name="destPath">destination content relative path</param>
    /// <returns></returns>
    [HttpGet]
    [Route("~/api/content/copy")]
    [Authorize(Permissions.Update)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> CopyContent([FromQuery] string srcPath, [FromQuery] string destPath)
    {
        await contentService.CopyContentAsync(null, null, srcPath, destPath);
        return NoContent();
    }

    /// <summary>
    /// Copy file
    /// </summary>
    /// <param name="contentType">type of content (pages/blogs/etc)</param>
    /// <param name="storeId">store id</param>
    /// <param name="srcFile">source file</param>
    /// <param name="destFile">source file</param>
    /// <returns></returns>
    [HttpPost]
    [Route("copy-file")]
    [Authorize(Permissions.Update)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> CopyFile(string contentType, string storeId, [FromQuery] string srcFile, [FromQuery] string destFile)
    {
        if (srcFile == null)
        {
            return NotFound();
        }
        if (destFile == null)
        {
            var ext = Path.GetExtension(srcFile);
            var alternativeExt = ext.EndsWith("-draft") ? ext.Substring(0, ext.Length - "-draft".Length) : ext + "-draft";
            var filename = Path.GetFileNameWithoutExtension(srcFile);
            var path = srcFile.Substring(0, srcFile.Length - filename.Length - ext.Length);
            var index = 0;
            do
            {
                index++;
            } while (
                await contentService.ItemExistsAsync(contentType, storeId, Path.Combine(path, $"{filename}_{index}{ext}"))
                || await contentService.ItemExistsAsync(contentType, storeId, Path.Combine(path, $"{filename}_{index}{alternativeExt}"))

            );
            destFile = Path.Combine(path, $"{filename}_{index}{ext}");
        }

        destFile = _publishingService.GetRelativeDraftUrl(destFile, true);
        await contentService.CopyFileAsync(contentType, storeId, srcFile, destFile);
        return NoContent();
    }

    /// <summary>
    ///     Unpack contents
    /// </summary>
    /// <param name="contentType">possible values Themes or Pages</param>
    /// <param name="storeId">Store id</param>
    /// <param name="archivePath">archive file relative path</param>
    /// <param name="destPath">destination content relative path</param>
    /// <returns></returns>
    [HttpGet]
    [Route("unpack")]
    [Authorize(Permissions.Update)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Unpack(string contentType, string storeId, [FromQuery] string archivePath, [FromQuery] string destPath = "default")
    {
        await contentService.UnpackAsync(contentType, storeId, archivePath, destPath);
        return NoContent();
    }

    /// <summary>
    ///     Create content folder
    /// </summary>
    /// <param name="contentType">possible values Themes or Pages</param>
    /// <param name="storeId">Store id</param>
    /// <param name="folder">content folder</param>
    /// <returns></returns>
    [HttpPost]
    [Route("folder")]
    [Authorize(Permissions.Create)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateContentFolder(string contentType, string storeId, [FromBody] ContentFolder folder)
    {
        var validation = await new ContentFolderValidator().ValidateAsync(folder);

        if (!validation.IsValid)
        {
            return BadRequest(new
            {
                Message = string.Join(" ", validation.Errors.Select(x => x.ErrorMessage)),
                Errors = validation.Errors,
            });
        }

        await contentService.CreateFolderAsync(contentType, storeId, folder);
        return NoContent();
    }

    /// <summary>
    ///     Upload content item
    /// </summary>
    /// <param name="contentType">possible values Themes or Pages</param>
    /// <param name="storeId">Store id</param>
    /// <param name="folderUrl">folder relative url where content will be uploaded</param>
    /// <param name="url">external url which will be used to download content item data</param>
    /// <param name="draft">Whether file should be saved as draft.</param>
    /// <returns></returns>
    [HttpPost]
    [Route("")]
    [DisableFormValueModelBinding]
    [Authorize(Permissions.Create)]
    [ProducesResponseType(typeof(void), StatusCodes.Status405MethodNotAllowed)]
    public async Task<ActionResult<ContentItem[]>> UploadContent(string contentType, string storeId, [FromQuery] string folderUrl, [FromQuery] string url = null, [FromQuery] bool draft = false)
    {
        if (url is null && !MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        {
            return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
        }

        var retVal = new List<ContentFile>();
        try
        {
            if (url is not null)
            {
                var file = await contentService.DownloadContentAsync(contentType, storeId, url, folderUrl);
                retVal.Add(file);
            }
            else
            {
                var headerContentType = MediaTypeHeaderValue.Parse(Request.ContentType);
                var boundary = MultipartRequestHelper.GetBoundary(headerContentType, _defaultFormOptions.MultipartBoundaryLengthLimit);
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);

                var section = await reader.ReadNextSectionAsync();
                if (section is null)
                {
                    throw new InvalidOperationException(nameof(section));
                }

                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    var fileName = Path.GetFileName(contentDisposition.FileName.Value ?? contentDisposition.Name.Value.Replace("\"", string.Empty));

                    fileName = _publishingService.GetRelativeDraftUrl(fileName, draft);

                    var file = await contentService.SaveContentAsync(contentType, storeId, folderUrl, fileName, section.Body);
                    retVal.Add(file);
                }
            }
        }
        catch (PlatformException ex)
        {
            return new ObjectResult(new { ex.Message }) { StatusCode = StatusCodes.Status405MethodNotAllowed };
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred while uploading the file. Error message: {error}", ex.Message);
            return BadRequest();
        }

        ContentCacheRegion.ExpireContent(($"content-{storeId}"));

        return Ok(retVal.ToArray());
    }

    [HttpPost]
    [Route("publishing")]
    [Authorize(Permissions.Create)]
    public async Task<ActionResult> Publishing(string contentType, string storeId, [FromQuery] string relativeUrl, [FromQuery] bool publish)
    {
        await _publishingService.PublishingAsync(contentType, storeId, relativeUrl, publish);
        return Ok();
    }

    [HttpGet]
    [Route("publish-status")]
    [Authorize(Permissions.Create)]
    public async Task<ActionResult<FilePublishStatus>> PublishStatus(string contentType, string storeId, [FromQuery] string relativeUrl)
    {
        var result = await _publishingService.PublishStatusAsync(contentType, storeId, relativeUrl);
        return Ok(result);
    }
}
