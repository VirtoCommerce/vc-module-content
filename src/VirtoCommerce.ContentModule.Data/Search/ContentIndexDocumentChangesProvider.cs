using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentIndexDocumentChangesProvider : IIndexDocumentChangesProvider
    {
        private readonly IContentFileService _contentFileService;
        private readonly ISearchService<StoreSearchCriteria, StoreSearchResult, Store> _storeService;
        private readonly IContentStatisticService _contentStatisticService;

        public ContentIndexDocumentChangesProvider(
            IContentFileService contentFileService,
            IStoreSearchService storeService,
            IContentStatisticService contentStatisticService)
        {
            _contentFileService = contentFileService;
            _storeService = (ISearchService<StoreSearchCriteria, StoreSearchResult, Store>)storeService;
            _contentStatisticService = contentStatisticService;
        }

        public virtual async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            var result = new List<IndexDocumentChange>();
            var now = DateTime.UtcNow;
            await ApplyToStores(async store =>
            {
                var filter = AbstractTypeFactory<FilterItemsCriteria>.TryCreateInstance();
                filter.ContentType = ContentConstants.ContentTypes.Pages;
                filter.StoreId = store.Id;
                var pages = await _contentFileService.EnumerateFiles(filter);
                var pagesToIndex = pages
                    .Where(file => file.ModifiedDate == null || (startDate == null || file.ModifiedDate >= startDate) && (endDate == null || file.ModifiedDate <= endDate))
                    .OrderByDescending(file => file.ModifiedDate ?? now)
                    .Skip((int)skip)
                    .Take((int)take)
                    .Select(file => new IndexDocumentChange
                    {
                        DocumentId = DocumentIdentifierHelper.GenerateId(store.Id, file),
                        ChangeType = IndexDocumentChangeType.Modified,
                        ChangeDate = file.ModifiedDate ?? now
                    }).ToArray();
                result.AddRange(pagesToIndex);
            });
            return result;
        }

        public virtual async Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
        {
            var result = 0;
            await ApplyToStores(async store =>
            {
                var storeId = store.Id;
                var pagesCount = await _contentStatisticService.GetStoreChangedPagesCountAsync(storeId, startDate, endDate);
                result += pagesCount;
            });

            return result;
        }

        private async Task ApplyToStores(Func<Store, Task> action)
        {
            const int take = 50;
            var skip = 0;
            int totalStores;
            do
            {
                var stores = await GetStores(skip, take);
                totalStores = stores.TotalCount;
                foreach (var store in stores.Results)
                {
                    await action(store);
                }

                skip += take;
            }
            while (skip < totalStores);
        }

        private async Task<StoreSearchResult> GetStores(int skip, int take)
        {
            var criteria = AbstractTypeFactory<StoreSearchCriteria>.TryCreateInstance();
            criteria.Skip = skip;
            criteria.Take = take;
            var stores = await _storeService.SearchAsync(criteria);
            return stores;
        }
    }
}
