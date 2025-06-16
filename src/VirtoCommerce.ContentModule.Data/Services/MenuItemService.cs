using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Events;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.ContentModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Caching;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.GenericCrud;

namespace VirtoCommerce.ContentModule.Data.Services;
public class MenuItemService(
    Func<IMenuRepository> repositoryFactory,
    IPlatformMemoryCache platformMemoryCache,
    IEventPublisher eventPublisher) : CrudService<MenuItem, MenuItemEntity, MenuItemChangingEvent, MenuItemChangedEvent>(repositoryFactory, platformMemoryCache, eventPublisher), IMenuItemService
{
    protected override Task<IList<MenuItemEntity>> LoadEntities(IRepository repository, IList<string> ids, string responseGroup)
    {
        return ((IMenuRepository)repository).GetMenuItems(ids);
    }
}
