using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Events;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Model;
using VirtoCommerce.ContentModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.ContentModule.Data.Services
{
    [Obsolete("Use IMenuLinkListService or IMenuLinkListSearchService", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
    public class MenuService : IMenuService
    {
        private readonly Func<IMenuRepository> _menuRepositoryFactory;
        private readonly IEventPublisher _eventPublisher;

        public MenuService(Func<IMenuRepository> menuRepositoryFactory, IEventPublisher eventPublisher)
        {
            _menuRepositoryFactory = menuRepositoryFactory;
            _eventPublisher = eventPublisher;
        }

        [Obsolete("Use IMenuLinkListSearchService.SearchAll()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task<IEnumerable<MenuLinkList>> GetAllLinkListsAsync()
        {
            using (var repository = _menuRepositoryFactory())
            {
                var entities = await repository.GetAllLinkListsAsync();
                return entities.Select(x => x.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance()));
            }
        }

        [Obsolete("Use IMenuLinkListSearchService.SearchAll()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task<IEnumerable<MenuLinkList>> GetListsByStoreIdAsync(string storeId)
        {
            using (var repository = _menuRepositoryFactory())
            {
                var entities = await repository.GetListsByStoreIdAsync(storeId);
                return entities.Select(x => x.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance()));
            }
        }

        [Obsolete("Use IMenuLinkListService.GetByIdAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task<MenuLinkList> GetListByIdAsync(string listId)
        {
            using (var repository = _menuRepositoryFactory())
            {
                var entities = await repository.GetListByIdAsync(listId);
                return entities.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance());
            }
        }

        [Obsolete("Use IMenuLinkListService.SaveChangesAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task AddOrUpdateAsync(MenuLinkList list)
        {
            using (var repository = _menuRepositoryFactory())
            {
                var changedEntries = new List<GenericChangedEntry<MenuLinkList>>();
                var pkMap = new PrimaryKeyResolvingMap();

                var targetEntity = await repository.GetListByIdAsync(list.Id);
                var sourceEntity = AbstractTypeFactory<MenuLinkListEntity>.TryCreateInstance().FromModel(list, pkMap);

                if (targetEntity != null)
                {
                    /// This extension is allow to get around breaking changes is introduced in EF Core 3.0 that leads to throw
                    /// Database operation expected to affect 1 row(s) but actually affected 0 row(s) exception when trying to add the new children entities with manually set keys
                    /// https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-3.0/breaking-changes#detectchanges-honors-store-generated-key-values
                    repository.TrackModifiedAsAddedForNewChildEntities(targetEntity);

                    changedEntries.Add(new GenericChangedEntry<MenuLinkList>(list, targetEntity.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance()), EntryState.Modified));
                    sourceEntity.Patch(targetEntity);
                }
                else
                {
                    repository.Add(sourceEntity);
                    changedEntries.Add(new GenericChangedEntry<MenuLinkList>(list, EntryState.Added));
                }

                await repository.UnitOfWork.CommitAsync();
                pkMap.ResolvePrimaryKeys();
                await _eventPublisher.Publish(new MenuLinkListChangedEvent(changedEntries));
            }
        }

        [Obsolete("Use IMenuLinkListService.DeleteAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task DeleteListAsync(string listId)
        {
            await DeleteListsAsync(new[] { listId });
        }

        [Obsolete("Use IMenuLinkListService.DeleteAsync()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task DeleteListsAsync(string[] listIds)
        {
            if (listIds == null)
            {
                throw new ArgumentNullException(nameof(listIds));
            }

            var changedEntries = new List<GenericChangedEntry<MenuLinkList>>();
            using (var repository = _menuRepositoryFactory())
            {
                foreach (var listId in listIds)
                {
                    var existList = await repository.GetListByIdAsync(listId);
                    if (existList != null)
                    {
                        changedEntries.Add(new GenericChangedEntry<MenuLinkList>(existList.ToModel(AbstractTypeFactory<MenuLinkList>.TryCreateInstance()), EntryState.Deleted));
                        repository.Remove(existList);
                    }
                }
                await repository.UnitOfWork.CommitAsync();

                await _eventPublisher.Publish(new MenuLinkListChangedEvent(changedEntries));

            }
        }

        [Obsolete("Use IMenuLinkListSearchService.IsNameUnique()", DiagnosticId = "VC0005", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public async Task<bool> CheckListAsync(string storeId, string name, string language, string id)
        {
            using (var repository = _menuRepositoryFactory())
            {
                var lists = await repository.GetListsByStoreIdAsync(storeId);

                var retVal = !lists.Any(l => string.Equals(l.Name, name, StringComparison.OrdinalIgnoreCase)
                                    && (l.Language == language || (string.IsNullOrEmpty(l.Language) && string.IsNullOrEmpty(language)))
                                    && l.Id != id);

                return retVal;
            }
        }
    }
}
