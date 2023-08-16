using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VirtoCommerce.ContentModule.Core.Extensions;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using Permissions = VirtoCommerce.ContentModule.Core.ContentConstants.Security.Permissions;

namespace VirtoCommerce.ContentModule.Web.Controllers.Api
{
    [Route("api/cms/{storeId}/menu")]
    public class MenuController : Controller
    {
        private readonly IMenuLinkListService _crudService;
        private readonly IMenuLinkListSearchService _searchService;

        public MenuController(IMenuLinkListService crudService, IMenuLinkListSearchService searchService)
        {
            _crudService = crudService;
            _searchService = searchService;
        }

        /// <summary>
        /// Get menu link lists
        /// </summary>
        /// <param name="storeId">Store id</param>
        [HttpGet]
        [Route("")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<MenuLinkList[]>> GetLists([FromRoute] string storeId)
        {
            var lists = await _searchService.SearchAllNoClone(storeId);

            if (lists.Any())
            {
                return Ok(lists);
            }

            return Ok();
        }

        /// <summary>
        /// Get menu link list by id
        /// </summary>
        /// <param name="listId">List id</param>
        [HttpGet]
        [Route("{listId}")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<MenuLinkList>> GetList([FromRoute] string listId)
        {
            var item = await _crudService.GetNoCloneAsync(listId);
            return Ok(item);
        }

        /// <summary>
        /// Checking name of menu link list
        /// </summary>
        /// <remarks>Checking pair of name+language of menu link list for unique, if checking result - false saving unavailable</remarks>
        /// <param name="storeId">Store id</param>
        /// <param name="name">Name of menu link list</param>
        /// <param name="language">Language of menu link list</param>
        /// <param name="id">Menu link list id</param>
        [HttpGet]
        [Route("checkname")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<bool>> CheckName([FromRoute] string storeId, [FromQuery] string name, [FromQuery] string language = "", [FromQuery] string id = "")
        {
            var retVal = await _searchService.IsNameUnique(storeId, name, language, id);
            return Ok(new { Result = retVal });
        }

        /// <summary>
        /// Update menu link list
        /// </summary>
        /// <param name="list">Menu link list</param>
        [HttpPost]
        [Route("")]
        [Authorize(Permissions.Update)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> UpdateMenuLinkList([FromBody] MenuLinkList list)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            await _crudService.SaveChangesAsync(new[] { list });
            return NoContent();
        }

        /// <summary>
        /// Delete menu link list
        /// </summary>
        /// <param name="listIds">Menu link list id</param>
        [HttpDelete]
        [Route("")]
        [Authorize(Permissions.Delete)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteMenuLinkLists([FromQuery] string[] listIds)
        {
            if (listIds == null)
            {
                throw new ArgumentNullException(nameof(listIds));
            }

            await _crudService.DeleteAsync(listIds);

            return NoContent();
        }
    }
}
