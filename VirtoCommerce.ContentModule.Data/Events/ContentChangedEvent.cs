using System.Collections.Generic;
using VirtoCommerce.Domain.Common.Events;
using VirtoCommerce.Platform.Core.Assets;

namespace VirtoCommerce.ContentModule.Data
{
    public class ContentChangedEvent : GenericChangedEntryEvent<BlobInfo>
    {
        public ContentChangedEvent(IEnumerable<GenericChangedEntry<BlobInfo>> changedEntries) : base(changedEntries)
        {
        }
    }
}
