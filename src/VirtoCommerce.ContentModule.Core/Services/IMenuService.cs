using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services
{
    [Obsolete("Use IMenuLinkListService or IMenuLinkListSearchService", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
    public interface IMenuService
    {
        [Obsolete("Use IMenuLinkListSearchService.SearchAll()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task<IEnumerable<MenuLinkList>> GetAllLinkListsAsync();

        [Obsolete("Use IMenuLinkListSearchService.SearchAll()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task<IEnumerable<MenuLinkList>> GetListsByStoreIdAsync(string storeId);

        [Obsolete("Use IMenuLinkListService.GetByIdAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task<MenuLinkList> GetListByIdAsync(string listId);

        [Obsolete("Use IMenuLinkListService.SaveChangesAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task AddOrUpdateAsync(MenuLinkList list);

        [Obsolete("Use IMenuLinkListService.DeleteAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task DeleteListAsync(string listId);

        [Obsolete("Use IMenuLinkListService.DeleteAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task DeleteListsAsync(string[] listIds);

        [Obsolete("Use IMenuLinkListSearchService.IsNameUnique()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task<bool> CheckListAsync(string storeId, string name, string language, string id);
    }
}
