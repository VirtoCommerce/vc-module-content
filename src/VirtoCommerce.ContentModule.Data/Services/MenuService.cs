using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class MenuService : IMenuService
    {
        private const int _batchSize = 50;

        private readonly IMenuLinkListService _menuLinkListService;
        private readonly IMenuLinkListSearchService _menuLinkListSearchService;

        public MenuService(IMenuLinkListService menuLinkListService, IMenuLinkListSearchService menuLinkListSearchService)
        {
            _menuLinkListService = menuLinkListService;
            _menuLinkListSearchService = menuLinkListSearchService;
        }

        [Obsolete("Use IMenuLinkListSearchService.SearchAsync()")]
        public async Task<IEnumerable<MenuLinkList>> GetAllLinkListsAsync()
        {
            return await GetAllListsAsync(storeId: null, clone: true);
        }

        public async Task<IEnumerable<MenuLinkList>> GetListsByStoreIdAsync(string storeId)
        {
            return await GetAllListsAsync(storeId, clone: true);
        }

        public Task<IList<MenuLinkList>> GetListsByStoreIdAsync(string storeId, bool clone)
        {
            return GetAllListsAsync(storeId, clone);
        }

        [Obsolete("Use IMenuLinkListService.GetByIdAsync()")]
        public Task<MenuLinkList> GetListByIdAsync(string listId)
        {
            return _menuLinkListService.GetByIdAsync(listId);
        }

        [Obsolete("Use IMenuLinkListService.SaveChangesAsync()")]
        public Task AddOrUpdateAsync(MenuLinkList list)
        {
            return _menuLinkListService.SaveChangesAsync(new[] { list });
        }

        [Obsolete("Use IMenuLinkListService.DeleteAsync()")]
        public Task DeleteListAsync(string listId)
        {
            return _menuLinkListService.DeleteAsync(new[] { listId });
        }

        [Obsolete("Use IMenuLinkListService.DeleteAsync()")]
        public Task DeleteListsAsync(string[] listIds)
        {
            return _menuLinkListService.DeleteAsync(listIds);
        }

        public async Task<bool> CheckListAsync(string storeId, string name, string language, string id)
        {
            var lists = await GetAllListsAsync(storeId, clone: false);

            return !lists.Any(x =>
                x.Name.EqualsInvariant(name) &&
                (x.Language == language || (string.IsNullOrEmpty(x.Language) && string.IsNullOrEmpty(language))) &&
                x.Id != id);
        }


        protected Task<IList<MenuLinkList>> GetAllListsAsync(string storeId, bool clone)
        {
            var criteria = AbstractTypeFactory<MenuLinkListSearchCriteria>.TryCreateInstance();
            criteria.StoreId = storeId;
            criteria.Take = _batchSize;

            return _menuLinkListSearchService.SearchAll(criteria, clone);
        }
    }
}
