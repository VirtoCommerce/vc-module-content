using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
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
using static Humanizer.In;
using Permissions = VirtoCommerce.ContentModule.Core.ContentConstants.Security.Permissions;

namespace VirtoCommerce.ContentModule.Web.Controllers.Api
{
    [Route("api/content/{contentType}/{storeId}")]
    public class ContentController : Controller
    {
        private readonly IBlobContentStorageProviderFactory _blobContentStorageProviderFactory;
        private readonly IPlatformMemoryCache _platformMemoryCache;
        private readonly ICrudService<Store> _storeService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ContentOptions _options;
        private readonly ILogger<ContentController> _logger;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();

        private const string _blogsFolderName = "blogs";
        private const string _pages = "pages";
        private const string _themes = "themes";
        private const string _defaultTheme = "default";

        public ContentController(
            IBlobContentStorageProviderFactory blobContentStorageProviderFactory,
            IPlatformMemoryCache platformMemoryCache,
            IStoreService storeService,
            IHttpClientFactory httpClientFactory,
            IOptions<ContentOptions> options,
            ILogger<ContentController> logger)
        {
            _blobContentStorageProviderFactory = blobContentStorageProviderFactory;
            _platformMemoryCache = platformMemoryCache;
            _storeService = (ICrudService<Store>)storeService;
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
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
            var contentStorageProvider = _blobContentStorageProviderFactory.CreateProvider("");
            var cacheKey = CacheKey.With(GetType(), "pagesCount", $"content-{storeId}");
            var pagesCount = _platformMemoryCache.GetOrCreateExclusive(cacheKey, cacheEntry =>
            {
                cacheEntry.AddExpirationToken(ContentCacheRegion.CreateChangeToken($"content-{storeId}"));
                var result = CountContentItemsRecursive(GetContentBasePath(_pages, storeId), contentStorageProvider, _blogsFolderName);
                return result;
            });

            var storeTask = _storeService.GetByIdAsync(storeId, StoreResponseGroup.DynamicProperties.ToString());
            var themesTask = contentStorageProvider.SearchAsync(GetContentBasePath(_themes, storeId), null);
            var blogsTask = contentStorageProvider.SearchAsync(GetContentBasePath(_blogsFolderName, storeId), null);

            await Task.WhenAll(themesTask, blogsTask, storeTask);

            var store = storeTask.Result;
            var themes = themesTask.Result;
            var blogs = blogsTask.Result;

            var retVal = new ContentStatistic
            {
                ActiveThemeName = store.DynamicProperties.FirstOrDefault(x => x.Name == "DefaultThemeName")?.Values?.FirstOrDefault()?.Value.ToString() ?? _defaultTheme,
                ThemesCount = themes.Results.OfType<BlobFolder>().Count(),
                BlogsCount = blogs.Results.OfType<BlobFolder>().Count(),
                PagesCount = pagesCount
            };

            return Ok(retVal);
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
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));
            await storageProvider.RemoveAsync(urls);

            //ToDo Reset cached items
            //_cacheManager.ClearRegion($"content-{storeId}");
            ContentCacheRegion.ExpireRegion();
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
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));
            if ((await storageProvider.GetBlobInfoAsync(relativeUrl)) != null)
            {
                var fileStream = storageProvider.OpenRead(relativeUrl);
                return File(fileStream, MimeTypeResolver.ResolveContentType(relativeUrl));
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
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));

            var searchResult = await storageProvider.SearchAsync(folderUrl, keyword);

            var result = searchResult.Results.OfType<BlobFolder>()
                .Select(x => x.ToContentModel())
                .OfType<ContentItem>()
                .Concat(searchResult.Results.OfType<BlobInfo>().Select(x => x.ToContentModel()))
                .Where(x => folderUrl != null || !x.Name.EqualsInvariant(_blogsFolderName))
                .ToArray();

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
        public ActionResult MoveContent(string contentType, string storeId, [FromQuery] string oldUrl, [FromQuery] string newUrl)
        {
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));

            storageProvider.Move(oldUrl, newUrl);
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
        public ActionResult CopyContent([FromQuery] string srcPath, [FromQuery] string destPath)
        {
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(string.Empty);

            //This method used only for default themes copying that we use string.Empty instead storeId because default themes placed only in root content folder
            storageProvider.Copy(srcPath, destPath);
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
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));

            using (var stream = storageProvider.OpenRead(archivePath))
            using (var archive = new ZipArchive(stream))
            {
                // count number of root folders, if one, we use our standard approach of ignoring root folder
                var foldersCount = archive.Entries.Where(x => x.FullName.Split('/').Length > 1 || x.FullName.EndsWith("/")).Select(f => f.FullName.Split('/')[0]).Distinct().Count();

                foreach (var entry in archive.Entries)
                    if (!entry.FullName.EndsWith("/"))
                    {
                        var fileName = foldersCount == 1 ? string.Join("/", entry.FullName.Split('/').Skip(1)) : entry.FullName;

                        using (var entryStream = entry.Open())
                        using (var targetStream = storageProvider.OpenWrite(destPath + "/" + fileName))
                        {
                            entryStream.CopyTo(targetStream);
                        }
                    }
            }

            //remove archive after unpack
            await storageProvider.RemoveAsync(new[] { archivePath });

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

            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));

            await storageProvider.CreateFolderAsync(folder.ToBlobModel(AbstractTypeFactory<BlobFolder>.TryCreateInstance()));

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
            var storageProvider = _blobContentStorageProviderFactory.CreateProvider(GetContentBasePath(contentType, storeId));
            try
            {
                if (url is not null)
                {
                    var fileName = HttpUtility.UrlDecode(Path.GetFileName(url));
                    var fileUrl = $"{folderUrl}/{fileName}";

                    using (var client = _httpClientFactory.CreateClient())
                    using (var blobStream = storageProvider.OpenWrite(fileUrl))
                    using (var remoteStream = await client.GetStreamAsync(url))
                    {
                        remoteStream.CopyTo(blobStream);

                        var сontentFile = AbstractTypeFactory<ContentFile>.TryCreateInstance();

                        сontentFile.Name = fileName;
                        сontentFile.Url = storageProvider.GetAbsoluteUrl(fileUrl);
                        retVal.Add(сontentFile);
                    }
                }
                else
                {
                    var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
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

                        var targetFilePath = $"{folderUrl}/{fileName}";

                        using (var targetStream = storageProvider.OpenWrite(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                        }

                        var contentFile = AbstractTypeFactory<ContentFile>.TryCreateInstance();
                        contentFile.Name = fileName;
                        contentFile.Url = storageProvider.GetAbsoluteUrl(targetFilePath);
                        retVal.Add(contentFile);
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

        private string GetContentBasePath(string contentType, string storeId)
        {
            if (_options.PathMappings != null && _options.PathMappings.Any() && _options.PathMappings.ContainsKey(contentType))
            {
                var themeName = _defaultTheme;
                var mapping = _options.PathMappings[contentType];
                var parts = mapping.Select(x => x switch
                {
                    "_storeId" => storeId,
                    "_theme" => themeName,
                    "_blog" => _blogsFolderName,
                    _ => x,
                });
                var result = string.Join('/', parts);
                return result;
            }

            var retVal = contentType switch
            {
                var x when x.EqualsInvariant(_themes) => "Themes/" + storeId,
                var x when x.EqualsInvariant(_pages) => "Pages/" + storeId,
                var x when x.EqualsInvariant(_blogsFolderName) => "Pages/" + storeId + $"/{_blogsFolderName}",
                var x => string.Empty
            };

            return retVal;
        }

        private static int CountContentItemsRecursive(string folderUrl, IBlobStorageProvider blobContentStorageProvider, string excludedFolderName = null)
        {
            var searchResult = blobContentStorageProvider.SearchAsync(folderUrl, null).GetAwaiter().GetResult();

            var folders = searchResult.Results.OfType<BlobFolder>();

            var retVal = searchResult.TotalCount - folders.Count()
                         + searchResult.Results.OfType<BlobFolder>()
                             .Where(x => string.IsNullOrEmpty(excludedFolderName) || !x.Name.EqualsInvariant(excludedFolderName))
                             .Select(x => CountContentItemsRecursive(x.RelativeUrl, blobContentStorageProvider))
                             .Sum();

            return retVal;
        }
    }
}
