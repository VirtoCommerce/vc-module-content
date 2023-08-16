using System;
using System.Collections.Generic;
using System.Linq;
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

public class MenuLinkListSearchService : SearchService<MenuLinkListSearchCriteria, MenuLinkListSearchResult, MenuLinkList, MenuLinkListEntity>, IMenuLinkListSearchService
{
    public MenuLinkListSearchService(
        Func<IMenuRepository> repositoryFactory,
        IPlatformMemoryCache platformMemoryCache,
        IMenuLinkListService crudService,
        IOptions<CrudOptions> crudOptions)
        : base(repositoryFactory, platformMemoryCache, crudService, crudOptions)
    {
    }

    protected override IQueryable<MenuLinkListEntity> BuildQuery(IRepository repository, MenuLinkListSearchCriteria criteria)
    {
        var query = ((IMenuRepository)repository).MenuLinkLists;

        if (criteria.StoreId != null)
        {
            query = query.Where(x => x.StoreId == criteria.StoreId);
        }

        if (criteria.Name != null)
        {
            query = query.Where(x => x.Name == criteria.Name);
        }

        return query;
    }

    protected override IList<SortInfo> BuildSortExpression(MenuLinkListSearchCriteria criteria)
    {
        var sortInfos = criteria.SortInfos;

        if (sortInfos.IsNullOrEmpty())
        {
            sortInfos = new[]
            {
                new SortInfo { SortColumn = nameof(MenuLinkListEntity.Name) },
            };
        }

        return sortInfos;
    }
}
