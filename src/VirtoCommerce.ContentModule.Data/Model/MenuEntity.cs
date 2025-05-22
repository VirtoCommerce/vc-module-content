using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Domain;

namespace VirtoCommerce.ContentModule.Data.Model;
public class MenuEntity : AuditableEntity, IHasOuterId, IDataEntity<MenuEntity, Menu>
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string StoreId { get; set; }

    public string Language { get; set; }
    [StringLength(128)]
    public string OuterId { get; set; }

    public virtual ObservableCollection<MenuItemEntity> Items { get; set; } = new();

    public Menu ToModel(Menu menu)
    {
        menu.Id = Id;
        menu.CreatedBy = CreatedBy;
        menu.CreatedDate = CreatedDate;
        menu.ModifiedBy = ModifiedBy;
        menu.ModifiedDate = ModifiedDate;

        menu.OuterId = OuterId;
        menu.Name = Name;
        menu.StoreId = StoreId;
        menu.Language = Language;

        if (Items.Any())
        {
            menu.Items = Items.OrderByDescending(l => l.Priority)
                .Select(s => s.ToModel(AbstractTypeFactory<MenuItem>.TryCreateInstance()))
                .ToArray();
        }

        return menu;
    }

    public MenuEntity FromModel(Menu menu, PrimaryKeyResolvingMap pkMap)
    {
        if (menu == null)
        {
            throw new ArgumentNullException(nameof(menu));
        }

        pkMap.AddPair(menu, this);

        Id = menu.Id;
        CreatedDate = menu.CreatedDate;
        CreatedBy = menu.CreatedBy;
        ModifiedDate = menu.ModifiedDate;
        ModifiedBy = menu.ModifiedBy;

        OuterId = menu.OuterId;
        StoreId = menu.StoreId;
        Name = menu.Name;
        Language = menu.Language;

        if (menu.Items != null)
        {
            Items = new ObservableCollection<MenuItemEntity>(menu.Items.Select(x => AbstractTypeFactory<MenuItemEntity>.TryCreateInstance().FromModel(x, pkMap)));
        }

        return this;
    }

    public void Patch(MenuEntity target)
    {
        target.Language = Language;
        target.Name = Name;
        target.StoreId = StoreId;

        if (!Items.IsNullCollection())
        {
            Items.Patch(target.Items, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
        }
    }
}
