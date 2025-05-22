using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model;
public class MenuItem : AuditableEntity, IHasOuterId, ICloneable
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
    public int Priority { get; set; }
    
    public string AssociatedObjectId { get; set; }
    public string AssociatedObjectName { get; set; }
    public string AssociatedObjectType { get; set; }

    public string MenuId { get; set; }
    public string MenuItemId { get; set; }
    public string OuterId { get; set; }

    public IList<MenuItem> Items { get; set; }

    public object Clone()
    {
        var result = MemberwiseClone() as MenuItem;

        return result;
    }
}
