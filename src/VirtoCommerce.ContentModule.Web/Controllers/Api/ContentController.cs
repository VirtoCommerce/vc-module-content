using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using VirtoCommerce.AssetsModule.Core.Assets;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Extensions;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.ContentModule.Web.Filters;
using VirtoCommerce.ContentModule.Web.Validators;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Exceptions;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.Helpers;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;
using Permissions = VirtoCommerce.ContentModule.Core.ContentConstants.Security.Permissions;
using UrlHelperExtensions = VirtoCommerce.Platform.Core.Extensions.UrlHelperExtensions;

namespace VirtoCommerce.ContentModule.Web.Controllers.Api
{
    [Route("api/content/{contentType}/{storeId}")]
    public class ContentController : Controller
    {
        private readonly IContentStatisticService _contentStats;
        private readonly IContentService _contentService;
        private readonly IContentSearchService _contentSearchService;
        private readonly ILogger<ContentController> _logger;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        public ContentController(
            IContentStatisticService contentStats,
            IContentService contentService,
            IContentSearchService contentSearchService,
            ILogger<ContentController> logger)
        {
            _contentStats = contentStats;
            _contentService = contentService;
            _contentSearchService = contentSearchService;
            _logger = logger;
        }

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
            var result = await _contentStats.GetStoreContentStatsAsync(storeId);
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
            await _contentService.DeleteContentAsync(contentType, storeId, urls);
            return NoContent();
        }

        /// <summary>
        ///     Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager)
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="relativeUrl">content relative url</param>
        /// <returns>stream</returns>
        [HttpGet]
        [Route("")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<byte[]>> GetContentItemDataStream(string contentType, string storeId, [FromQuery] string relativeUrl)
        {
            if (await _contentService.ItemExistsAsync(contentType, storeId, relativeUrl))
            {
                var result = await _contentService.GetItemStreamAsync(contentType, storeId, relativeUrl);
                return File(result, MimeTypeResolver.ResolveContentType(relativeUrl));
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
            var result = await _contentSearchService.FilterContentAsync(contentType, storeId, folderUrl, keyword);
            return Ok(result);
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
            await _contentService.MoveContentAsync(contentType, storeId, oldUrl, newUrl);
            return NoContent();
        }

        /// <summary>
        ///     Copy contents
        /// </summary>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/content/copy")]
        [Authorize(Permissions.Update)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> CopyContent([FromQuery] string srcPath, [FromQuery] string destPath)
        {
            await _contentService.CopyContentAsync(null, null, srcPath, destPath);
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
            await _contentService.UnpackAsync(contentType, storeId, archivePath, destPath);
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
            var validation = new ContentFolderValidator().Validate(folder);

            if (!validation.IsValid)
            {
                return BadRequest(new
                {
                    Message = string.Join(" ", validation.Errors.Select(x => x.ErrorMessage)),
                    Errors = validation.Errors
                });
            }

            await _contentService.CreateFolderAsync(contentType, storeId, folder);
            return NoContent();
        }

        /// <summary>
        ///     Upload content item
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">folder relative url where content will be uploaded</param>
        /// <param name="url">external url which will be used to download content item data</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [DisableFormValueModelBinding]
        [Authorize(Permissions.Create)]
        [ProducesResponseType(typeof(void), StatusCodes.Status405MethodNotAllowed)]
        public async Task<ActionResult<ContentItem[]>> UploadContent(string contentType, string storeId, [FromQuery] string folderUrl, [FromQuery] string url = null)
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
                    var file = await _contentService.DownloadContentAsync(contentType, storeId, url, folderUrl);
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
                        var file = await _contentService.SaveContentAsync(contentType, storeId, folderUrl, fileName, section.Body);
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
                _logger.LogError("An error occurred while uploading the file. Error message: {error}", ex.Message);
                return BadRequest();
            }

            ContentCacheRegion.ExpireContent(($"content-{storeId}"));

            return Ok(retVal.ToArray());
        }
    }
}
