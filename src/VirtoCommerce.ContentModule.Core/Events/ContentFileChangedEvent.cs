using System.Collections.Generic;
using Newtonsoft.Json;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.ContentModule.Core.Events;

public class ContentFileChangedEvent : GenericChangedEntryEvent<ContentFile>
{
    [JsonConstructor]
    public ContentFileChangedEvent(string contentType, string storeId, IEnumerable<GenericChangedEntry<ContentFile>> changedEntries)
        : base(changedEntries)
    {
        ContentType = contentType;
        StoreId = storeId;
    }

    public string ContentType { get; }
    public string StoreId { get; }
}
