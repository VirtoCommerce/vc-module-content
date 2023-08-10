using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Repositories
{
    public interface IMenuRepository : IRepository
    {
        public IQueryable<MenuLinkListEntity> MenuLinkLists { get; }

        Task<IList<MenuLinkListEntity>> GetListsByIdsAsync(IList<string> listIds);

        [Obsolete("Use GetListsByIdsAsync()")]
        Task<IEnumerable<MenuLinkListEntity>> GetAllLinkListsAsync();

        [Obsolete("Use GetListsByIdsAsync()")]
        Task<IEnumerable<MenuLinkListEntity>> GetListsByStoreIdAsync(string storeId);

        [Obsolete("Use GetListsByIdsAsync()")]
        Task<MenuLinkListEntity> GetListByIdAsync(string listId);
    }
}
