using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Data.Services;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Assets;

namespace VirtoCommerce.ContentModule.Data.Search.Indexing
{
    public class PagesDocumentChangesProvider : IIndexDocumentChangesProvider
    {
        private readonly Func<string, IContentBlobStorageProvider> _contentStorageProviderFactory;

        public PagesDocumentChangesProvider(Func<string, IContentBlobStorageProvider> contentStorageProviderFactory)
        {
            _contentStorageProviderFactory = contentStorageProviderFactory;
        }

        public virtual Task<long> GetTotalChangesCountAsync(DateTime? startDate, DateTime? endDate)
        {
            long result = 0;

            //if (startDate == null && endDate == null)
            //{
            // Get total products count
            //using (var repository = _memberRepositoryFactory())
            //{
            //    result = repository.Members.Count();
            //}
            //}
            //else
            //{
            //    // Get changes count from operation log
            //    result = _changeLogService.FindChangeHistory(ChangeLogObjectType, startDate, endDate).Count();
            //}

            return Task.FromResult(result);
        }

        public virtual Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            IList<IndexDocumentChange> result = null;

            //if (startDate == null && endDate == null)
            //{
            // Get documents from repository and return them as changes
            //using (var repository = _memberRepositoryFactory())
            //{
            //    var productIds = repository.Members
            //        .OrderBy(i => i.CreatedDate)
            //        .Select(i => i.Id)
            //        .Skip((int)skip)
            //        .Take((int)take)
            //        .ToArray();

            //    result = productIds.Select(id =>
            //        new IndexDocumentChange
            //        {
            //            DocumentId = id,
            //            ChangeType = IndexDocumentChangeType.Modified,
            //            ChangeDate = DateTime.UtcNow
            //        }
            //    ).ToArray();
            //}
            //}
            //else
            //{
            //    // Get changes from operation log
            //    var operations = _changeLogService.FindChangeHistory(ChangeLogObjectType, startDate, endDate)
            //        .Skip((int)skip)
            //        .Take((int)take)
            //        .ToArray();

            //    result = operations.Select(o =>
            //        new IndexDocumentChange
            //        {
            //            DocumentId = o.ObjectId,
            //            ChangeType = o.OperationType == EntryState.Deleted ? IndexDocumentChangeType.Deleted : IndexDocumentChangeType.Modified,
            //            ChangeDate = o.ModifiedDate ?? o.CreatedDate,
            //        }
            //    ).ToArray();
            //}

            return Task.FromResult(result);
        }

        private static ICollection<string> GetItemUrls(IContentBlobStorageProvider storageProvider, BlobSearchResult searchResult)
        {
            var urls = new List<string>();

            foreach (var item in searchResult.Items)
            {
                urls.Add(item.RelativeUrl);
            }
            foreach (var folder in searchResult.Folders)
            {
                var folderSearchResult = storageProvider.Search(folder.RelativeUrl, null);
                urls.AddRange(GetItemUrls(storageProvider, folderSearchResult));
            }

            return urls;
        }
    }
}
