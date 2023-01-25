using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.Platform.Data.GenericCrud;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model.Search;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentIndexDocumentChangesProvider : IIndexDocumentChangesProvider
    {
        private readonly IContentSearchService _contentSearchService;
        private readonly ISearchService<StoreSearchCriteria, StoreSearchResult, Store> _storeService;
        private readonly IContentStatisticService _contentStatisticService;

        public ContentIndexDocumentChangesProvider(
            IContentSearchService contentSearchService,
            IStoreSearchService storeService,
            IContentStatisticService contentStatisticService
        )
        {
            _contentSearchService = contentSearchService;
            _storeService = (ISearchService<StoreSearchCriteria, StoreSearchResult, Store>)storeService;
            _contentStatisticService = contentStatisticService;
        }

        // todo: use parameters
        public virtual async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            var stores = await GetStores();
            var result = new List<IndexDocumentChange>();
            var now = DateTime.UtcNow;
            foreach (var store in stores)
            {
                var pages = await _contentSearchService.EnumerateItems(ContentConstants.ContentTypes.Pages, store.Id, null);
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
            }
            return result;
        }

        // todo: use parameters
        public virtual async Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
        {
            var stores = await GetStores();
            var result = 0;
            foreach (var store in stores)
            {
                var storeId = store.Id;
                var pagesCount = await _contentStatisticService.GetStorePagesCountAsync(storeId);
                result += pagesCount;
            }
            return result;
        }

        private async Task<IEnumerable<Store>> GetStores()
        {
            var stores = await _storeService.SearchAsync(new StoreSearchCriteria
            {
                ObjectType = nameof(Store)
            });
            return stores.Results;
        }
    }
}
