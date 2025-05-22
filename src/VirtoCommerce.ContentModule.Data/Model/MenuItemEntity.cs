using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.ContentModule.Data.Model;
public class MenuItemEntity : AuditableEntity, IHasOuterId, IDataEntity<MenuItemEntity, MenuItem>
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
    public int Priority { get; set; }

    public string AssociatedObjectId { get; set; }
    public string AssociatedObjectName { get; set; }
    public string AssociatedObjectType { get; set; }

    public string OuterId { get; set; }

    public IList<MenuItemEntity> Items { get; set; } = new NullCollection<MenuItemEntity>();

    #region Navigation properties

    public string MenuId { get; set; }
    public virtual MenuEntity Menu { get; set; }

    public string ParentMenuItemId { get; set; }
    public virtual MenuItemEntity ParentMenuItem { get; set; }

    #endregion

    public MenuItem ToModel(MenuItem menuItem)
    {
        menuItem.Id = Id;
        menuItem.CreatedBy = CreatedBy;
        menuItem.CreatedDate = CreatedDate;
        menuItem.ModifiedBy = ModifiedBy;
        menuItem.ModifiedDate = ModifiedDate;

        menuItem.Name = Name;
        menuItem.Type = Type;
        menuItem.Url = Url;
        menuItem.Priority = Priority;
        menuItem.AssociatedObjectId = AssociatedObjectId;
        menuItem.AssociatedObjectName = AssociatedObjectName;
        menuItem.AssociatedObjectType = AssociatedObjectType;

        if (Items.Any())
        {
            menuItem.Items = Items.OrderByDescending(l => l.Priority)
                .Select(s => s.ToModel(AbstractTypeFactory<MenuItem>.TryCreateInstance()))
                .ToArray();
        }

        return menuItem;
    }

    public MenuItemEntity FromModel(MenuItem menuItem, PrimaryKeyResolvingMap pkMap)
    {
        if (menuItem == null)
        {
            throw new ArgumentNullException(nameof(menuItem));
        }

        pkMap.AddPair(menuItem, this);

        Id = menuItem.Id;
        CreatedDate = menuItem.CreatedDate;
        CreatedBy = menuItem.CreatedBy;
        ModifiedDate = menuItem.ModifiedDate;
        ModifiedBy = menuItem.ModifiedBy;

        Name = menuItem.Name;
        Type = menuItem.Type;
        Url = menuItem.Url;
        Priority = menuItem.Priority;
        AssociatedObjectId = menuItem.AssociatedObjectId;
        AssociatedObjectName = menuItem.AssociatedObjectName;
        AssociatedObjectType = menuItem.AssociatedObjectType;

        if (menuItem.Items != null)
        {
            Items = new ObservableCollection<MenuItemEntity>(menuItem.Items.Select(x => AbstractTypeFactory<MenuItemEntity>.TryCreateInstance().FromModel(x, pkMap)));
        }

        return this;
    }

    public void Patch(MenuItemEntity target)
    {
        target.Name = Name;
        target.Type = Type;
        target.Url = Url;
        target.Priority = Priority;
        target.AssociatedObjectId = AssociatedObjectId;
        target.AssociatedObjectName = AssociatedObjectName;
        target.AssociatedObjectType = AssociatedObjectType;

        if (!Items.IsNullCollection())
        {
            Items.Patch(target.Items, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
        }
    }
}
