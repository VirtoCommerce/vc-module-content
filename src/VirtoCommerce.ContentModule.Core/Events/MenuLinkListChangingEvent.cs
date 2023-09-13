using System.Collections.Generic;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.ContentModule.Core.Events;

public class MenuLinkListChangingEvent : GenericChangedEntryEvent<MenuLinkList>
{
    public MenuLinkListChangingEvent(IEnumerable<GenericChangedEntry<MenuLinkList>> changedEntries)
        : base(changedEntries)
    {
    }
}
