using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using VirtoCommerce.ContentModule.Data.Search;
using VirtoCommerce.Domain.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.ContentModule.Data.Handlers
{
    public class IndexContentChangedEventHandler : IEventHandler<ContentChangedEvent>
    {
        private readonly IIndexingManager _indexingManager;

        public IndexContentChangedEventHandler(IIndexingManager indexingManager)
        {
            _indexingManager = indexingManager;
        }

        public Task Handle(ContentChangedEvent message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var updatedIds = message.ChangedEntries.Where(x => (x.EntryState == EntryState.Modified || x.EntryState == EntryState.Added) && x.OldEntry.Key != null)
                                                          .Select(x => x.OldEntry.Key)
                                                          .Distinct().ToArray();

            if (!updatedIds.IsNullOrEmpty())
            {
                BackgroundJob.Enqueue(() => TryIndexBackgroundJob(updatedIds));
            }

            var deletedIds = message.ChangedEntries.Where(x => x.EntryState == EntryState.Deleted && x.OldEntry.Key != null)
                                                          .Select(x => x.OldEntry.Key)
                                                          .Distinct().ToArray();

            if (!deletedIds.IsNullOrEmpty())
            {
                BackgroundJob.Enqueue(() => TryDeleteIndexBackgroundJob(deletedIds));
            }

            return Task.CompletedTask;
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public Task TryIndexBackgroundJob(string[] updatedIds)
        {
            return TryIndex(updatedIds);
        }

        [DisableConcurrentExecution(60 * 60 * 24)]
        public Task TryDeleteIndexBackgroundJob(string[] deletedIds)
        {
            return TryDeleteIndex(deletedIds);
        }


        protected virtual Task TryIndex(string[] updatedIds)
        {
            return _indexingManager.IndexDocumentsAsync(ContentKnownDocumentTypes.MarkdownPages, updatedIds);
        }

        protected virtual Task TryDeleteIndex(string[] deletedIds)
        {
            return _indexingManager.DeleteDocumentsAsync(ContentKnownDocumentTypes.MarkdownPages, deletedIds);
        }

    }
}
