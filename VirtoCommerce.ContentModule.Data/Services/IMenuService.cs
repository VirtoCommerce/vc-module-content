using System.Collections.Generic;
using VirtoCommerce.ContentModule.Data.Models;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public interface IMenuService
    {
        IEnumerable<MenuLinkList> GetAllLinkLists();
        IEnumerable<MenuLinkList> GetListsByStoreId(string storeId);
        MenuLinkList GetListById(string listId);
        void AddOrUpdate(MenuLinkList list);
        void DeleteList(string listId);
        bool CheckList(string storeId, string name, string language, string id);
    }
}
