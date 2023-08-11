using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Extensions;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuLinkListService _menuLinkListService;
        private readonly IMenuLinkListSearchService _menuLinkListSearchService;

        public MenuService(IMenuLinkListService menuLinkListService, IMenuLinkListSearchService menuLinkListSearchService)
        {
            _menuLinkListService = menuLinkListService;
            _menuLinkListSearchService = menuLinkListSearchService;
        }

        [Obsolete("Use IMenuLinkListSearchService.SearchAll()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task<IEnumerable<MenuLinkList>> GetAllLinkListsAsync()
        {
            return await _menuLinkListSearchService.SearchAll();
        }

        [Obsolete("Use IMenuLinkListSearchService.SearchAll()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task<IEnumerable<MenuLinkList>> GetListsByStoreIdAsync(string storeId)
        {
            return await _menuLinkListSearchService.SearchAll(storeId);
        }

        [Obsolete("Use IMenuLinkListService.GetByIdAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public Task<MenuLinkList> GetListByIdAsync(string listId)
        {
            return _menuLinkListService.GetByIdAsync(listId);
        }

        [Obsolete("Use IMenuLinkListService.SaveChangesAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public Task AddOrUpdateAsync(MenuLinkList list)
        {
            return _menuLinkListService.SaveChangesAsync(new[] { list });
        }

        [Obsolete("Use IMenuLinkListService.DeleteAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public Task DeleteListAsync(string listId)
        {
            return _menuLinkListService.DeleteAsync(new[] { listId });
        }

        [Obsolete("Use IMenuLinkListService.DeleteAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public Task DeleteListsAsync(string[] listIds)
        {
            return _menuLinkListService.DeleteAsync(listIds);
        }

        [Obsolete("Use IMenuLinkListSearchService.IsNameUnique()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public Task<bool> CheckListAsync(string storeId, string name, string language, string id)
        {
            return _menuLinkListSearchService.IsNameUnique(storeId, name, language, id);
        }
    }
}
