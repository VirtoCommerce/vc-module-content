using System;
using System.Collections.Generic;
using System.Linq;
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
            long result;

            var storageProvider = _contentStorageProviderFactory(string.Empty);
            result = CountContentItemsRecursive("pages", storageProvider, startDate, endDate);

            return Task.FromResult(result);
        }

        public virtual Task<IList<IndexDocumentChange>> GetChangesAsync(DateTime? startDate, DateTime? endDate, long skip, long take)
        {
            IList<IndexDocumentChange> result = null;

            var storageProvider = _contentStorageProviderFactory(string.Empty);
            var searchResult = storageProvider.Search("pages", null);
            var blobInfos = GetItemInfos(storageProvider, searchResult);

            var itemUrls = blobInfos
                .Where(x => (startDate == null || x.ModifiedDate >= startDate) && (endDate == null || x.ModifiedDate <= endDate))
                .OrderBy(x => x.ModifiedDate)
                .Select(x => x.Key)
                .Skip((int)skip)
                .Take((int)take)
                .ToArray();

            result = itemUrls.Select(x =>
                new IndexDocumentChange
                {
                    DocumentId = x,
                    ChangeType = IndexDocumentChangeType.Modified,
                    ChangeDate = DateTime.UtcNow
                }
            ).ToArray();

            return Task.FromResult(result);
        }

        //TechDebt: CopyPaste from ContentController
        private int CountContentItemsRecursive(string folderUrl, IContentBlobStorageProvider _contentStorageProvider, DateTime? startDate, DateTime? endDate, string excludedFolderUrl = null)
        {
            var searchResult = _contentStorageProvider.Search(folderUrl, null);
            var retVal = searchResult.Items.Count(x => (startDate == null || x.ModifiedDate >= startDate) && (endDate == null || x.ModifiedDate <= endDate))
                        + searchResult.Folders
                            .Where(x => excludedFolderUrl == null || !x.RelativeUrl.EndsWith(excludedFolderUrl, StringComparison.InvariantCultureIgnoreCase))
                            .Select(x => CountContentItemsRecursive(x.RelativeUrl, _contentStorageProvider, startDate, endDate))
                            .Sum();

            return retVal;
        }

        //TechDebt: CopyPaste GetItemUrls from sitemaps module StaticContentSitemapItemRecordProvider 
        private static ICollection<BlobInfo> GetItemInfos(IContentBlobStorageProvider storageProvider, BlobSearchResult searchResult)
        {
            var urls = new List<BlobInfo>();

            foreach (var item in searchResult.Items)
            {
                urls.Add(item);
            }
            foreach (var folder in searchResult.Folders)
            {
                var folderSearchResult = storageProvider.Search(folder.RelativeUrl, null);
                urls.AddRange(GetItemInfos(storageProvider, folderSearchResult));
            }

            return urls;
        }
    }
}
