using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IMenuService
    {
        [Obsolete("Use IMenuLinkListSearchService.SearchAsync()")]
        Task<IEnumerable<MenuLinkList>> GetAllLinkListsAsync();

        Task<IEnumerable<MenuLinkList>> GetListsByStoreIdAsync(string storeId);

        Task<IList<MenuLinkList>> GetListsByStoreIdAsync(string storeId, bool clone);

        [Obsolete("Use IMenuLinkListService.GetByIdAsync()")]
        Task<MenuLinkList> GetListByIdAsync(string listId);

        [Obsolete("Use IMenuLinkListService.SaveChangesAsync()")]
        Task AddOrUpdateAsync(MenuLinkList list);

        [Obsolete("Use IMenuLinkListService.DeleteAsync()")]
        Task DeleteListAsync(string listId);

        [Obsolete("Use IMenuLinkListService.DeleteAsync()")]
        Task DeleteListsAsync(string[] listIds);

        Task<bool> CheckListAsync(string storeId, string name, string language, string id);
    }
}
