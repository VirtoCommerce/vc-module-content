using System;
using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model
{
    public class Menu : AuditableEntity, ICloneable
    {
        /// <summary>
        /// Name of menu link list, can be used as title of list in frontend
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Store identifier, for which the list belongs
        /// </summary>
        public string StoreId { get; set; }
        /// <summary>
        /// Locale of this menu link list
        /// </summary>
        public string Language { get; set; }

        public ICollection<MenuItem> Items { get; set; }

        public string[] SecurityScopes { get; set; }
        public string OuterId { get; set; }

        #region ICloneable members

        public virtual object Clone()
        {
            var result = MemberwiseClone() as Menu;

            result.Items = Items?.Select(x => x.Clone()).OfType<MenuItem>().ToList();

            return result;
        }

        #endregion
    }
}
