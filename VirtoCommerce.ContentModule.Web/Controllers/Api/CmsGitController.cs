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
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.ContentModule.Web.CmsGit;
using VirtoCommerce.ContentModule.Web.Converters;
using VirtoCommerce.ContentModule.Web.Models;
using VirtoCommerce.ContentModule.Web.Security;
using VirtoCommerce.Domain.Store.Services;
using VirtoCommerce.Platform.Core.Assets;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.DynamicProperties;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.Platform.Core.Web.Assets;
using VirtoCommerce.Platform.Core.Web.Security;
using VirtoCommerce.Platform.Data.Common;

namespace VirtoCommerce.ContentModule.Web.Controllers.Api
{
    [RoutePrefix("api/cmsgit/{storeId}")]
    public class CmsGitController : ContentBaseController
    {
        private readonly Func<string, IContentBlobStorageProvider> _contentStorageProviderFactory;
        private readonly IBlobUrlResolver _urlResolver;
        private readonly IStoreService _storeService;
        private readonly ICacheManager<object> _cacheManager;

        public CmsGitController(Func<string, IContentBlobStorageProvider> contentStorageProviderFactory, IBlobUrlResolver urlResolver, ISecurityService securityService, IPermissionScopeService permissionScopeService, IStoreService storeService, ICacheManager<object> cacheManager)
            : base(securityService, permissionScopeService)
        {
            _storeService = storeService;
            _contentStorageProviderFactory = contentStorageProviderFactory;
            _urlResolver = urlResolver;
            _cacheManager = cacheManager;
        }

        public class JsonPageBlockParam
        {
            public string content { get; set; }
        }

        /// <summary>
        /// Creates or saves the file in the git repository for the user
        /// </summary>
        /// <param name="storeId">Store id</param>
        /// <param name="userName">User name</param>
        /// <param name="fileName">File name</param>
        /// <param name="data">Content</param>
        /// <returns>unique link</returns>
        [HttpPost]
        [Route("~/api/cmsgit/{storeId}/{userName}/{fileName}/set")]
        [ResponseType(typeof(string))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Create)]
        public IHttpActionResult SetCmsGitFile(string storeId, string userName, string fileName, [FromBody] JsonPageBlockParam data)
        {
            var retVal = "ok";

            LocalGitRepository rep = new LocalGitRepository();

            rep.SetFile(userName, fileName, data.content);

            return Ok(retVal);
        }

        /// <summary>
        /// Get the file from the git repository for the user
        /// </summary>
        /// <param name="storeId">Store id</param>
        /// <param name="userName">User name</param>
        /// <param name="fileName">File name</param>
        /// <returns>unique link</returns>
        [HttpGet]
        [Route("~/api/cmsgit/{storeId}/{userName}/{fileName}/get")]
        [ResponseType(typeof(string))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Read)]
        public IHttpActionResult GetCmsGitFile(string storeId, string userName, string fileName)
        {
            var retVal = String.Empty;

            LocalGitRepository rep = new LocalGitRepository();

            retVal = rep.GetFile(userName, fileName);

            return Ok(retVal);
        }

        /// <summary>
        /// Checks whether the file exists or not
        /// </summary>
        /// <param name="storeId">Store id</param>
        /// <param name="userName">User name</param>
        /// <param name="fileName">File name</param>
        /// <returns>unique link</returns>
        [HttpGet]
        [Route("~/api/cmsgit/{storeId}/{userName}/{fileName}/exists")]
        [ResponseType(typeof(string))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Read)]
        public IHttpActionResult CheckCmsGitFileExists(string storeId, string userName, string fileName)
        {
            LocalGitRepository rep = new LocalGitRepository();

            var retVal = rep.FileExists(userName, fileName);

            return Ok(retVal);
        }

        /// <summary>
        /// Checks whether the permalink is unique within the repository
        /// </summary>
        /// <param name="storeId">Store id</param>
        /// <param name="userName">User name</param>
        /// <param name="fileName">User name</param>
        /// <param name="permalink">Permalink</param>
        /// <returns>unique link</returns>
        [HttpGet]
        [Route("~/api/cmsgit/{storeId}/{userName}/{fileName}/{permalink}/isunique")]
        [ResponseType(typeof(string))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Read)]
        public IHttpActionResult CheckCmsGitPermalinkUnique(string storeId, string userName, string fileName, string permalink)
        {
            LocalGitRepository rep = new LocalGitRepository();

            var retVal = rep.PermalinkUnique(userName, fileName, permalink);

            return Ok(retVal);
        }

        /// <summary>
        /// Moves the file to review
        /// </summary>
        /// <param name="storeId">Store id</param>
        /// <param name="userName">User name</param>
        /// <param name="fileName">File name</param>
        /// <returns>unique link</returns>
        [HttpPost]
        [Route("~/api/cmsgit/{storeId}/{userName}/{fileName}/sendToReview")]
        [ResponseType(typeof(string))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Update)]
        public IHttpActionResult SendToReview(string storeId, string userName, string fileName)
        {
            var retVal = "ok";

            LocalGitRepository rep = new LocalGitRepository();

            rep.MoveTo(userName, fileName, rep.BranchDraftName);

            return Ok(retVal);
        }

        /// <summary>
        /// Moves the file to production
        /// </summary>
        /// <param name="storeId">Store id</param>
        /// <param name="userName">User name</param>
        /// <param name="fileName">File name</param>
        /// <returns>unique link</returns>
        [HttpPost]
        [Route("~/api/cmsgit/{storeId}/{userName}/{fileName}/sendToProduction")]
        [ResponseType(typeof(string))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Update)]
        public IHttpActionResult SendToProduction(string storeId, string userName, string fileName)
        {
            var retVal = "ok";
            
            LocalGitRepository rep = new LocalGitRepository();

            rep.MoveTo(rep.BranchDraftName, fileName, rep.BranchMasterName);

            return Ok(retVal);
        }

        /// <summary>
        /// Update production folder
        /// </summary>
        /// <param name="storeId">Store id</param>
        /// <returns>unique link</returns>
        [HttpPost]
        [Route("~/api/cmsgit/{storeId}/refreshProduction")]
        [ResponseType(typeof(string))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Update)]
        public IHttpActionResult refreshProduction(string storeId)
        {
            var retVal = "ok";

            LocalGitRepository rep = new LocalGitRepository();

            rep.Update(rep.BranchMasterName);

            return Ok(retVal);
        }

        /// <summary>
        /// Update specific folder
        /// </summary>
        /// <param name="storeId">Store id</param>
        /// <param name="userName">User name</param>
        /// <returns>unique link</returns>
        [HttpPost]
        [Route("~/api/cmsgit/{storeId}/{userName}/refresh")]
        [ResponseType(typeof(string))]
        [CheckPermission(Permission = ContentPredefinedPermissions.Update)]
        public IHttpActionResult refresh(string storeId, string userName)
        {
            var retVal = "ok";

            LocalGitRepository rep = new LocalGitRepository();

            rep.Update(userName);

            return Ok(retVal);
        }

    }
}
