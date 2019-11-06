
using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.ContentModule.Web.Models
{
    public class ContentSearchCriteria : SearchCriteriaBase
    {
        public string StoreId { get; set; }

        public string ContentType { get; set; }

        public string FolderUrl { get; set; }

    }
}
