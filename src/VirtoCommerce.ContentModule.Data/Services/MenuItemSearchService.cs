using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.ContentModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.ContentModule.Data.Services;
public class MenuItemSearchService : SearchService<MenuItemSearchCriteria, MenuItemSearchResult, MenuItem, MenuItemEntity>, IMenuItemSearchService
{
    public MenuItemSearchService(
        Func<IMenuRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        IMenuItemService crudService,
        IOptions<CrudOptions> crudOptions)
        : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
    {
    }

    protected override IQueryable<MenuItemEntity> BuildQuery(IRepository repository, MenuItemSearchCriteria criteria)
    {
        var query = ((IMenuRepository)repository).MenuItems;

        if (criteria.StoreId != null)
        {
            query = query.Where(x => x.StoreId == criteria.StoreId);
        }

        if (criteria.Type != null)
        {
            query = query.Where(x => x.Type == criteria.Type);
        }

        if (criteria.Name != null)
        {
            query = query.Where(x => x.Name == criteria.Name);
        }

        return query;
    }

    protected override IList<SortInfo> BuildSortExpression(MenuItemSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            sortInfos = new[]
            {
                new SortInfo { SortColumn = nameof(MenuItemEntity.Name) },
            };
        }

        return sortInfos;
    }
}

