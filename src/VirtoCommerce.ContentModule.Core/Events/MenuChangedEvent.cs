using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Events;

namespace VirtoCommerce.ContentModule.Core.Events;
public class MenuChangedEvent : GenericChangedEntryEvent<Menu>
{
    public MenuChangedEvent(IEnumerable<GenericChangedEntry<Menu>> changedEntries) : base(changedEntries)
    {
    }
}
