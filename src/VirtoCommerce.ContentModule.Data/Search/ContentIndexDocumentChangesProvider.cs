using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
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
        private readonly IStoreSearchService _storeSearchService;
        private readonly IContentStatisticService _contentStatisticService;

        public ContentIndexDocumentChangesProvider(
            IContentFileService contentFileService,
            IStoreSearchService storeSearchService,
            IContentStatisticService contentStatisticService)
        {
            _contentFileService = contentFileService;
            _storeSearchService = storeSearchService;
            _contentStatisticService = contentStatisticService;
        }

        public virtual async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            var result = new List<IndexDocumentChange>();
            var now = DateTime.UtcNow;
            await ApplyToStores(async store =>
            {
                var pages = await GetFiles(store, ContentConstants.ContentTypes.Pages, now);
                var posts = await GetFiles(store, ContentConstants.ContentTypes.Blogs, now);
                var pagesToIndex = pages.Union(posts)
                    .Where(x => x.File.ModifiedDate == null || (startDate == null || x.File.ModifiedDate >= startDate) && (endDate == null || x.File.ModifiedDate <= endDate))
                    .OrderByDescending(x => x.File.ModifiedDate ?? now)
                    .Skip((int)skip)
                    .Take((int)take)
                    .Select(x => x.Document);
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
            var stores = await _storeSearchService.SearchStoresAsync(criteria);
            return stores;
        }

        private async Task<IList<(ContentFile File, IndexDocumentChange Document)>> GetFiles(Store store, string contentType, DateTime now)
        {
            var filter = AbstractTypeFactory<FilterItemsCriteria>.TryCreateInstance();
            filter.ContentType = contentType;
            filter.StoreId = store.Id;
            var files = await _contentFileService.EnumerateFiles(filter);
            return files.Select(x => (x, new IndexDocumentChange
            {
                DocumentId = DocumentIdentifierHelper.GenerateId(store.Id, contentType, x),
                ChangeType = IndexDocumentChangeType.Modified,
                ChangeDate = x.ModifiedDate ?? now,
            })).ToList();
        }
    }
}
