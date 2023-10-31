using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model
{
    public class ContentSearchCriteria : SearchCriteriaBase
    {
        public string StoreId { get; set; }
        public string CultureName { get; set; }
        public string FolderUrl { get; set; }
    }
}
