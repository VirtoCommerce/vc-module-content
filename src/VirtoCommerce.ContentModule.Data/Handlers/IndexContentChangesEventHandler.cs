using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Events;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.ContentModule.Data.Search;
using VirtoCommerce.Platform.Core.Events;
using VirtoCommerce.Platform.Core.PushNotifications;
using VirtoCommerce.Platform.Core.Security;
using VirtoCommerce.SearchModule.Core.BackgroundJobs;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ContentModule.Data.Handlers;

public class IndexContentChangesEventHandler : IEventHandler<ContentFileChangedEvent>
{
    private readonly IIndexingJobService _indexingJobService;
    private readonly IUserNameResolver _userNameResolver;
    private readonly IPushNotificationManager _pushNotifier;
    private readonly IPublishingService _publishingService;

    public IndexContentChangesEventHandler(
        IIndexingJobService indexingJobService,
        IUserNameResolver userNameResolver,
        IPushNotificationManager pushNotifier,
        IPublishingService publishingService)
    {
        _indexingJobService = indexingJobService;
        _userNameResolver = userNameResolver;
        _pushNotifier = pushNotifier;
        _publishingService = publishingService;
    }

    public Task Handle(ContentFileChangedEvent message)
    {
        var options = new IndexingOptions[]
        {
            new ()
            {
                DocumentType = FullTextContentSearchService.ContentDocumentType,
                DocumentIds = message
                    .ChangedEntries
                    .Select(x => GetDocumentId(message.StoreId, message.ContentType, x))
                    .Where(x => x != null)
                    .ToArray(),
            }
        };


        if (options.Any())
        {
            var currentUserName = _userNameResolver.GetCurrentUserName();
            var notification = _indexingJobService.Enqueue(currentUserName, options.ToArray());
            _pushNotifier.Send(notification);
        }

        return Task.CompletedTask;
    }

    private string GetDocumentId(string storeId, string contentType, GenericChangedEntry<ContentFile> changes)
    {
        // There are 4 situations possible:
        // 1. User creates or saves a draft file, old entry is null, new entry is draft - nothing to index
        // 2. User deletes a file, old entry is published file, new entry is null - we need to remove it from the index
        // 3. User unpublishes a file, old entry is published file, new entry is draft - we need to remove it from the index
        // 4. User publishes a draft file, old entry is a draft, new entry is published file - we need to index it

        var oldDraft = changes.OldEntry != null && _publishingService.IsDraft(changes.OldEntry.RelativeUrl);
        var newDraft = changes.NewEntry != null && _publishingService.IsDraft(changes.NewEntry.RelativeUrl);

        if (newDraft)
        {
            return null;
        }

        if (changes.NewEntry == null && !oldDraft)
        {
            // how to delete?
            return null;
        }

        return DocumentIdentifierHelper.GenerateId(storeId, contentType, changes.NewEntry);
    }
}
