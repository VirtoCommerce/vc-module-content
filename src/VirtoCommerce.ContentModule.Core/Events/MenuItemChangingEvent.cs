using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.ContentModule.Core.Events;
public class MenuItemChangingEvent : GenericChangedEntryEvent<MenuItem>
{
    public MenuItemChangingEvent(IEnumerable<GenericChangedEntry<MenuItem>> changedEntries) : base(changedEntries)
    {
    }
}
