using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model
{
    public class ContentSearchCriteria : SearchCriteriaBase
    {
        public string StoreId { get; set; }
        public string CultureName { get; set; }
        public string FolderUrl { get; set; }
        public string ContentType { get; set; }
        public string OrganizationId { get; set; }
        public string[] UserGroups { get; set; }
        public DateTime? ActiveOn { get; set; }
    }
}
