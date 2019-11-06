using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using CacheManager.Core;
using VirtoCommerce.ContentModule.Data;
using VirtoCommerce.ContentModule.Data.Search;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.ContentModule.Data.Utility;
using VirtoCommerce.ContentModule.Web.Converters;
using VirtoCommerce.ContentModule.Web.Models;
using VirtoCommerce.ContentModule.Web.Security;

using VirtoCommerce.Domain.Common.Events;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Web.Assets;
using VirtoCommerce.Platform.Core.Web.Security;
using VirtoCommerce.Platform.Data.Common;

namespace VirtoCommerce.ContentModule.Web.Controllers.Api
{
    [RoutePrefix("api/content/{contentType}/{storeId}")]
    public class ContentController : ContentBaseController
    {
        private readonly Func<string, IContentBlobStorageProvider> _contentStorageProviderFactory;
        private readonly IBlobUrlResolver _urlResolver;
        private readonly IStoreService _storeService;
        private readonly ICacheManager<object> _cacheManager;
        private readonly IContentSearchService _contentSearchService;
        private readonly IEventPublisher _eventPublisher;

        public ContentController(Func<string, IContentBlobStorageProvider> contentStorageProviderFactory,
            IBlobUrlResolver urlResolver,
            ISecurityService securityService,
            IPermissionScopeService permissionScopeService,
            IStoreService storeService,
            ICacheManager<object> cacheManager,
            IContentSearchService contentSearchService,
            IEventPublisher eventPublisher)
            : base(securityService, permissionScopeService)
        {
            _storeService = storeService;
            _contentStorageProviderFactory = contentStorageProviderFactory;
            _urlResolver = urlResolver;
            _cacheManager = cacheManager;
            _contentSearchService = contentSearchService;
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// Return summary content statistic 
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns>Object contains counters with main content types</returns>
        [HttpGet]
        [Route("~/api/content/{storeId}/stats")]
        [ResponseType(typeof(ContentStatistic))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Read)]
        public IHttpActionResult GetStoreContentStats(string storeId)
        {
            var contentStorageProvider = _contentStorageProviderFactory("");
            var store = _storeService.GetById(storeId);

            var pagesCount = _cacheManager.Get("pagesCount", $"content-{storeId}", TimeSpan.FromMinutes(1), () =>
           {
               return CountContentItemsRecursive(ContentTypeUtility.GetContentBasePath("pages", storeId), contentStorageProvider, ContentTypeUtility.GetContentBasePath("blogs", storeId)); ;
           });

            var retVal = new ContentStatistic
            {
                ActiveThemeName = store.GetDynamicPropertyValue("DefaultThemeName", "not set"),
                ThemesCount = contentStorageProvider.Search(ContentTypeUtility.GetContentBasePath("themes", storeId), null).Folders.Count,
                BlogsCount = contentStorageProvider.Search(ContentTypeUtility.GetContentBasePath("blogs", storeId), null).Folders.Count,
                PagesCount = pagesCount
            };
            return Ok(retVal);
        }


        /// <summary>
        /// Delete content from server
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id </param>
        /// <param name="urls">relative content urls to delete</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Delete)]
        public async Task<IHttpActionResult> DeleteContent(string contentType, string storeId, [FromUri] string[] urls)
        {
            var storageProvider = _contentStorageProviderFactory(ContentTypeUtility.GetContentBasePath(contentType, storeId));
            var changedEntries = new List<GenericChangedEntry<BlobInfo>>();

            foreach (var url in urls)
            {
                var fileName = HttpUtility.UrlDecode(System.IO.Path.GetFileName(url));
                var blobInfo = new BlobInfo
                {
                    ContentType = contentType,
                    Url = _urlResolver.GetAbsoluteUrl(url),
                    FileName = fileName,
                    RelativeUrl = url,
                    Key = url
                };
                changedEntries.Add(new GenericChangedEntry<BlobInfo>(blobInfo, EntryState.Deleted));
            }
            await _eventPublisher.Publish(new ContentChangedEvent(changedEntries));

            storageProvider.Remove(urls);
            _cacheManager.ClearRegion($"content-{storeId}");
            return Ok();
        }

        /// <summary>
        /// Return streamed data for requested by relativeUrl content (Used to prevent Cross domain requests in manager)
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="relativeUrl">content relative url</param>
        /// <returns>stream</returns>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(byte[]))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Read)]
        public HttpResponseMessage GetContentItemDataStream(string contentType, string storeId, string relativeUrl)
        {
            var storageProvider = _contentStorageProviderFactory(ContentTypeUtility.GetContentBasePath(contentType, storeId));
            var stream = storageProvider.OpenRead(relativeUrl);
            var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(relativeUrl));
            return result;
        }


        /// <summary>
        /// Search content items in specified folder and using search keyword
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">relative path for folder where content items will be searched</param>
        /// <param name="keyword">search keyword</param>
        /// <returns>content items</returns>
        [HttpGet]
        [Route("search")]
        [ResponseType(typeof(ContentItem[]))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Read)]
        public IHttpActionResult SearchContent(string contentType, string storeId, string folderUrl = null, string keyword = null)
        {
            var storageProvider = _contentStorageProviderFactory(ContentTypeUtility.GetContentBasePath(contentType, storeId));

            var result = storageProvider.Search(folderUrl, keyword);
            var retVal = result.Folders.Select(x => x.ToContentModel())
                               .OfType<ContentItem>()
                               .Concat(result.Items.Select(x => x.ToContentModel()))
                               .ToArray();
            return Ok(retVal);
        }

        /// <summary>
        /// Search content items by specified search criteria
        /// </summary>
        /// <param name="criteria">searching criteria</param>
        /// <returns></returns>
        [HttpPost]
        [Route("search")]
        [ResponseType(typeof(ContentSearchResult))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Read)]
        public IHttpActionResult SearchContent(ContentSearchCriteria criteria)
        {
            var result = _contentSearchService.SearchAsync(criteria);
            return Ok(result);
        }

        /// <summary>
        /// Rename or move content item
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="oldUrl">old content item relative or absolute url</param>
        /// <param name="newUrl">new content item relative or absolute url</param>
        /// <returns></returns>
        [HttpGet]
        [Route("move")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Update)]
        public async Task<IHttpActionResult> MoveContent(string contentType, string storeId, string oldUrl, string newUrl)
        {
            var storageProvider = _contentStorageProviderFactory(ContentTypeUtility.GetContentBasePath(contentType, storeId));

            var changedEntries = new List<GenericChangedEntry<BlobInfo>>();

            var fileName = HttpUtility.UrlDecode(System.IO.Path.GetFileName(newUrl));
            var blobInfoNew = new BlobInfo
            {
                ContentType = contentType,
                Url = _urlResolver.GetAbsoluteUrl(newUrl),
                FileName = fileName,
                RelativeUrl = newUrl,
                Key = newUrl
            };

            fileName = HttpUtility.UrlDecode(System.IO.Path.GetFileName(oldUrl));
            var blobInfoOld = new BlobInfo
            {
                ContentType = contentType,
                Url = _urlResolver.GetAbsoluteUrl(oldUrl),
                FileName = fileName,
                RelativeUrl = oldUrl,
                Key = oldUrl
            };
            changedEntries.Add(new GenericChangedEntry<BlobInfo>(blobInfoOld, EntryState.Deleted));
            changedEntries.Add(new GenericChangedEntry<BlobInfo>(blobInfoNew, EntryState.Added));

            await _eventPublisher.Publish(new ContentChangedEvent(changedEntries));

            storageProvider.MoveContent(oldUrl, newUrl);
            return Ok();
        }

        /// <summary>
        /// Copy contents
        /// </summary>
        /// <param name="srcPath">source content  relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/content/copy")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Update)]
        public IHttpActionResult CopyContent(string srcPath, string destPath)
        {
            //This method used only for default themes copying that we use string.Empty instead storeId because default themes placed only in root content folder
            var storageProvider = _contentStorageProviderFactory(string.Empty);

            storageProvider.CopyContent(srcPath, destPath);
            return Ok();
        }

        /// <summary>
        /// Unpack contents
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="archivePath">archive file relative path</param>
        /// <param name="destPath">destination content relative path</param>
        /// <returns></returns>
        [HttpGet]
        [Route("unpack")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Update)]
        public IHttpActionResult Unpack(string contentType, string storeId, string archivePath, string destPath)
        {
            var storageProvider = _contentStorageProviderFactory(ContentTypeUtility.GetContentBasePath(contentType, storeId));

            using (var stream = storageProvider.OpenRead(archivePath))
            using (var archive = new ZipArchive(stream))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.FullName.EndsWith("/"))
                    {
                        var fileName = String.Join("/", entry.FullName.Split('/').Skip(1));
                        using (var entryStream = entry.Open())
                        using (var targetStream = storageProvider.OpenWrite(destPath + "/" + fileName))
                        {
                            entryStream.CopyTo(targetStream);
                        }
                    }
                }

            }
            //remove archive after unpack
            storageProvider.Remove(new[] { archivePath });
            return Ok();
        }

        /// <summary>
        /// Create content folder 
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folder">content folder</param>
        /// <returns></returns>
        [HttpPost]
        [Route("folder")]
        [ResponseType(typeof(void))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Create)]
        public IHttpActionResult CreateContentFolder(string contentType, string storeId, ContentFolder folder)
        {
            var storageProvider = _contentStorageProviderFactory(ContentTypeUtility.GetContentBasePath(contentType, storeId));

            storageProvider.CreateFolder(folder.ToBlobModel());
            return Ok();
        }


        /// <summary>
        /// Upload content item 
        /// </summary>
        /// <param name="contentType">possible values Themes or Pages</param>
        /// <param name="storeId">Store id</param>
        /// <param name="folderUrl">folder relative url where content will be uploaded</param>
        /// <param name="url">external url which will be used to download content item data</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(ContentItem[]))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Create)]
        public async Task<IHttpActionResult> UploadContent(string contentType, string storeId, [FromUri] string folderUrl, [FromUri]string url = null)
        {

            if (url == null && !Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var retVal = new List<ContentFile>();
            var storageProvider = _contentStorageProviderFactory(ContentTypeUtility.GetContentBasePath(contentType, storeId));

            if (url != null)
            {
                var fileName = HttpUtility.UrlDecode(System.IO.Path.GetFileName(url));
                var fileUrl = folderUrl + "/" + fileName;
                using (var client = new WebClient())
                using (var blobStream = storageProvider.OpenWrite(fileUrl))
                using (var remoteStream = client.OpenRead(url))
                {
                    remoteStream.CopyTo(blobStream);

                    retVal.Add(new ContentFile
                    {
                        Name = fileName,
                        Url = _urlResolver.GetAbsoluteUrl(fileUrl)
                    });

                    var changedEntry = new GenericChangedEntry<BlobInfo>(
                        new BlobInfo
                        {
                            ContentType = contentType,
                            Url = _urlResolver.GetAbsoluteUrl(fileUrl),
                            FileName = fileName,
                            RelativeUrl = fileUrl,
                            Size = remoteStream.Length,
                            Key = fileUrl
                        }, EntryState.Added);

                    await _eventPublisher.Publish(new ContentChangedEvent(new List<GenericChangedEntry<BlobInfo>>() { changedEntry }));
                }
            }
            else
            {
                var blobMultipartProvider = new BlobStorageMultipartProvider(storageProvider, _urlResolver, folderUrl);
                await Request.Content.ReadAsMultipartAsync(blobMultipartProvider);

                var files = blobMultipartProvider.BlobInfos.Select(blobInfo => new ContentFile
                {
                    Name = blobInfo.FileName,
                    Url = _urlResolver.GetAbsoluteUrl(blobInfo.Key)
                });

                var changedEntries = blobMultipartProvider.BlobInfos.Select(blobInfo => new GenericChangedEntry<BlobInfo>(blobInfo, EntryState.Added));
                await _eventPublisher.Publish(new ContentChangedEvent(changedEntries));
                retVal.AddRange(files);
            }

            _cacheManager.ClearRegion($"content-{storeId}");
            return Ok(retVal.ToArray());
        }

        private int CountContentItemsRecursive(string folderUrl, IContentBlobStorageProvider _contentStorageProvider, string excludedFolderUrl = null)
        {
            var searchResult = _contentStorageProvider.Search(folderUrl, null);
            var retVal = searchResult.Items.Count
                        + searchResult.Folders
                            .Where(x => excludedFolderUrl == null || !x.RelativeUrl.EndsWith(excludedFolderUrl, StringComparison.InvariantCultureIgnoreCase))
                            .Select(x => CountContentItemsRecursive(x.RelativeUrl, _contentStorageProvider))
                            .Sum();

            return retVal;
        }
    }
}
