using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Models
{
    public class MenuLinkList : AuditableEntity
    {
        public MenuLinkList()
        {
            MenuLinks = new ObservableCollection<MenuLink>();
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string StoreId { get; set; }
        
        public string Language { get; set; }
        public virtual ObservableCollection<MenuLink> MenuLinks { get; set; }
    }
}
