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
    [Route("api/cms/{storeId}/new-menu")]
    public class NewMenuController : Controller
    {
        private readonly IMenuService _crudService;
        private readonly IMenuSearchService _searchService;

        public NewMenuController(IMenuService crudService, IMenuSearchService searchService)
        {
            _crudService = crudService;
            _searchService = searchService;
        }

        [HttpGet]
        [Route("")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<Menu[]>> GetMenus([FromRoute] string storeId, string responseGroup)
        {
            var lists = await _searchService.SearchAllNoClone(storeId);

            if (lists.Any())
            {
                return Ok(lists);
            }

            return Ok();
        }

        [HttpGet]
        [Route("{menuId}")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<Menu>> GetMenu([FromRoute] string menuId)
        {
            var item = await _crudService.GetNoCloneAsync(menuId);
            return Ok(item);
        }

        [HttpGet]
        [Route("checkname")]
        [Authorize(Permissions.Read)]
        public async Task<ActionResult<bool>> CheckName([FromRoute] string storeId, [FromQuery] string name, [FromQuery] string language = "", [FromQuery] string id = "")
        {
            var retVal = await _searchService.IsNameUnique(storeId, name, language, id);
            return Ok(new { Result = retVal });
        }

        [HttpPost]
        [Route("")]
        [Authorize(Permissions.Update)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> CreateMenu([FromBody] Menu menu)
        {
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu));
            }

            await _crudService.SaveChangesAsync([menu]);
            return NoContent();
        }

        [HttpPut]
        [Route("")]
        [Authorize(Permissions.Update)]
        public async Task<ActionResult<Menu>> UpdateMenu([FromBody] Menu menu)
        {
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu));
            }

            await _crudService.SaveChangesAsync([menu]);
            return Ok(menu);
        }

        [HttpDelete]
        [Route("")]
        [Authorize(Permissions.Delete)]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteMenus([FromQuery] string[] menuIds)
        {
            if (menuIds == null)
            {
                throw new ArgumentNullException(nameof(menuIds));
            }

            await _crudService.DeleteAsync(menuIds);
            return NoContent();
        }
    }
}
