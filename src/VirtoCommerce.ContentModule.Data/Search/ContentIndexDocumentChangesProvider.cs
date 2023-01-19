using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.SearchModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Services;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentIndexDocumentChangesProvider : IIndexDocumentChangesProvider
    {
        private readonly IContentSearchService _contentSearchService;
        private readonly ICrudService<Store> _storeService;
        private readonly IContentStatisticService _contentStatistcService;

        public ContentIndexDocumentChangesProvider(
            IContentSearchService contentSearchService,
            IStoreService storeService,
            IContentStatisticService contentStatistcService
        )
        {
            _contentSearchService = contentSearchService;
            _storeService = (ICrudService<Store>)storeService;
            _contentStatistcService = contentStatistcService;
        }

        public virtual async Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            var storeId = "B2B-store";
            // todo: search in every store
            var pages = await _contentSearchService.EnumerateItems(ContentConstants.ContentTypes.Pages, storeId, null);

            var result = pages.Select(file =>
                    new IndexDocumentChange
                    {
                        DocumentId = file.RelativeUrl,
                        ChangeType = IndexDocumentChangeType.Modified,
                        ChangeDate = DateTime.UtcNow
                    }
                ).ToArray();
            return result;
        }

        public virtual async Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
        {
            var storeId = "B2B-store";
            // todo: search in every store
            var result = await _contentStatistcService.GetStoreContentStatsAsync(storeId);
            return result.PagesCount;
        }
    }
}
