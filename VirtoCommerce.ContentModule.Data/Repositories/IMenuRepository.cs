using System.Collections.Generic;
using VirtoCommerce.ContentModule.Data.Models;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Repositories
{
    public interface IMenuRepository : IRepository
    {
        IEnumerable<MenuLinkList> GetAllLinkLists();
        IEnumerable<MenuLinkList> GetListsByStoreId(string storeId);
        MenuLinkList GetListById(string listId);
    }
}
