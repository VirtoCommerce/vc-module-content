using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.ContentModule.Data.Repositories
{
    public class MenuRepository : DbContextRepositoryBase<MenuDbContext>, IMenuRepository
    {
        public MenuRepository(MenuDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<MenuLinkListEntity> MenuLinkLists => DbContext.Set<MenuLinkListEntity>();

        public IQueryable<MenuLinkEntity> MenuLinks => DbContext.Set<MenuLinkEntity>();

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
    }
}
