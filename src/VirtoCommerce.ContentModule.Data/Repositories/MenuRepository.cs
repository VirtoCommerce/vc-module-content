using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.ContentModule.Data.Repositories
{
    public class MenuRepository : DbContextRepositoryBase<MenuDbContext>, IMenuRepository
    {
        public MenuRepository(MenuDbContext dbContext)
            : base(dbContext)
        {
            dbContext.SavingChanges += OnSavingChanges;
        }

        public IQueryable<MenuLinkListEntity> MenuLinkLists => DbContext.Set<MenuLinkListEntity>();

        public IQueryable<MenuLinkEntity> MenuLinks => DbContext.Set<MenuLinkEntity>();

        public IQueryable<MenuItemEntity> MenuItems => DbContext.Set<MenuItemEntity>();

        public async Task<IList<MenuLinkListEntity>> GetListsByIdsAsync(IList<string> ids)
        {
            var lists = await MenuLinkLists
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();

            if (lists.Any())
            {
                var existingIds = lists.Select(x => x.Id).ToList();
                await MenuLinks.Where(x => existingIds.Contains(x.MenuLinkListId)).LoadAsync();
            }

            return lists;
        }

        public virtual async Task<IList<MenuItemEntity>> GetMenuItems(IList<string> ids)
        {
            var menus = await MenuItems
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();

            if (menus.Count != 0)
            {
                var existingIds = menus.Select(x => x.Id).ToList();
                await LoadMenuItemsInner(existingIds);
            }

            return menus;
        }

        public virtual async Task LoadMenuItemsInner(IList<string> ids)
        {
            var items = await MenuItems.Where(x => ids.Contains(x.ParentMenuItemId)).ToListAsync();

            if (items.Count != 0)
            {
                var existingIds = items.Select(x => x.Id).ToList();
                await LoadMenuItemsInner(existingIds);
            }
        }

        [Obsolete("Use GetListsByIdsAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task<IEnumerable<MenuLinkListEntity>> GetAllLinkListsAsync()
        {
            return await MenuLinkLists.Include(m => m.MenuLinks).ToArrayAsync();
        }

        [Obsolete("Use GetListsByIdsAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task<MenuLinkListEntity> GetListByIdAsync(string listId)
        {
            return await MenuLinkLists.Include(m => m.MenuLinks).Where(m => m.Id == listId).FirstOrDefaultAsync();
        }

        [Obsolete("Use GetListsByIdsAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task<IEnumerable<MenuLinkListEntity>> GetListsByStoreIdAsync(string storeId)
        {
            return await MenuLinkLists.Include(m => m.MenuLinks).Where(m => m.StoreId == storeId).ToArrayAsync();
        }

        private void OnSavingChanges(object sender, SavingChangesEventArgs args)
        {
            var ctx = (DbContext)sender;
            var entries = ctx.ChangeTracker.Entries();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified &&
                    IsOrphanedEntity(entry))
                {
                    entry.State = EntityState.Deleted;
                }
            }
        }

        protected virtual bool IsOrphanedEntity(EntityEntry entry)
        {
            return entry.Entity is MenuItemEntity { ParentMenuItemId: null, Type: not "Menu" };
        }
    }
}
