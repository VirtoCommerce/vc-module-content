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

        Task<IList<MenuLinkListEntity>> GetListsByIdsAsync(IList<string> ids);

        [Obsolete("Use GetListsByIdsAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task<IEnumerable<MenuLinkListEntity>> GetAllLinkListsAsync();

        [Obsolete("Use GetListsByIdsAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task<IEnumerable<MenuLinkListEntity>> GetListsByStoreIdAsync(string storeId);

        [Obsolete("Use GetListsByIdsAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        Task<MenuLinkListEntity> GetListByIdAsync(string listId);
    }
}
