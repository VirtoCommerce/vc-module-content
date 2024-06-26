using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Events;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.SearchModule.Core.BackgroundJobs;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ContentModule.Data.Handlers;

public class IndexContentChangesEventHandler : IEventHandler<ContentFileChangedEvent>
{
    private readonly IIndexingJobService _indexingJobService;
    private readonly IPublishingService _publishingService;

    public IndexContentChangesEventHandler(
        IIndexingJobService indexingJobService,
        IPublishingService publishingService)
    {
        _indexingJobService = indexingJobService;
        _publishingService = publishingService;
    }

    public Task Handle(ContentFileChangedEvent message)
    {
        var entries = message.ChangedEntries
            .Select(x => GetIndexEntry(message.StoreId, message.ContentType, x))
            .Where(x => x != null)
            .ToArray();

        _indexingJobService.EnqueueIndexAndDeleteDocuments(entries);

        return Task.CompletedTask;
    }

    private IndexEntry GetIndexEntry(string storeId, string contentType, GenericChangedEntry<ContentFile> entry)
    {
        // There are 4 situations possible:
        // 1. User deletes a file, old entry is published file, new entry is null - we need to remove it from the index
        // 2. User unpublishes a file, old entry is published file, new entry is draft - we need to remove it from the index
        // 3. User creates or saves a draft file, old entry is null, new entry is draft - nothing to index
        // 4. User publishes a draft file, old entry is a draft, new entry is published file - we need to index it

        var oldPublished = entry.OldEntry != null && !_publishingService.IsDraft(entry.OldEntry.RelativeUrl);
        var newPublished = entry.NewEntry != null && !_publishingService.IsDraft(entry.NewEntry.RelativeUrl);

        if (oldPublished && !newPublished)
        {
            return new IndexEntry
            {
                Id = DocumentIdentifierHelper.GenerateId(storeId, contentType, entry.OldEntry),
                EntryState = EntryState.Deleted,
                Type = FullTextContentSearchService.ContentDocumentType,
            };
        }

        if (newPublished)
        {
            return new IndexEntry
            {
                Id = DocumentIdentifierHelper.GenerateId(storeId, contentType, entry.NewEntry),
                EntryState = EntryState.Modified,
                Type = FullTextContentSearchService.ContentDocumentType,
            };
        }

        return null;
    }
}
