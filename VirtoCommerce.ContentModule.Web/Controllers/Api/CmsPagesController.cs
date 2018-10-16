//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http;
//using System.Web.Http.Cors;
//using System.Web.Http.Description;
//using CacheManager.Core;
//using VirtoCommerce.ContentModule.Data.Services;
//using VirtoCommerce.ContentModule.Web.Security;
//using VirtoCommerce.Domain.Store.Services;
//using VirtoCommerce.Platform.Core.Assets;
//using VirtoCommerce.Platform.Core.Common;
//using VirtoCommerce.Platform.Core.Security;
//using VirtoCommerce.Platform.Core.Web.Security;

//namespace VirtoCommerce.ContentModule.Web.Controllers.Api
//{
//    /// <summary>
//    /// contentType - type of content (blog/news/article)
//    /// storeId - id of given store (Clothing/Electronics)
//    /// </summary>
//    [RoutePrefix("api/content/cms/{contentType}/{storeId}")]
//    public class CmsPagesController : ContentBaseController
//    {
//        private readonly Func<string, IContentBlobStorageProvider> _contentStorageProviderFactory;

//        public CmsPagesController(
//                Func<string, IContentBlobStorageProvider> contentStorageProviderFactory,
//                // IBlobUrlResolver urlResolver,
//                ISecurityService securityService,
//                IPermissionScopeService permissionScopeService
//                // IStoreService storeService,
//                // ICacheManager<object> cacheManager
//            ) : base(securityService, permissionScopeService)
//        {
//            this._contentStorageProviderFactory = contentStorageProviderFactory;
//        }

//        [HttpPost]
//        [Route("")]
//        [CheckPermission(Permission = ContentPredefinedPermissions.Update)]
//        [EnableCors(origins: "*", headers: "*", methods: "*")]
//        public async Task<IHttpActionResult> Post(string contentType, string storeId, string relativeUrl)
//        {
//            var givenStream = await Request.Content.ReadAsStreamAsync();
//            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));
//            var targetStream = storageProvider.OpenWrite(relativeUrl);
//            await givenStream.CopyToAsync(targetStream);
//            targetStream.Close();
//            return Ok();
//        }

//        [HttpGet]
//        [Route("")]
//        [ResponseType(typeof(byte[]))]
//        [CheckPermission(Permission = ContentPredefinedPermissions.Read)]
//        [EnableCors(origins: "*", headers: "*", methods: "*")]
//        public HttpResponseMessage Get(string contentType, string storeId, string relativeUrl)
//        {
//            var storageProvider = _contentStorageProviderFactory(GetContentBasePath(contentType, storeId));
//            var stream = storageProvider.OpenRead(relativeUrl);
//            var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(stream) };
//            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
//            return result;
//        }

//        private string GetContentBasePath(string contentType, string storeId)
//        {
//            var retVal = string.Empty;
//            if (contentType.EqualsInvariant("pages"))
//            {
//                retVal = "Pages/" + storeId + "/pages";
//            }
//            return retVal;
//        }
//    }
//}
