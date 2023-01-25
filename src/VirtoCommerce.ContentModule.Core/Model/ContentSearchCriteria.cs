using System.Diagnostics.Metrics;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model
{
    public class ContentSearchCriteria : SearchCriteriaBase
    {
        public ContentSearchCriteria()
        {
            ObjectType = nameof(ContentFile);
        }

        public string StoreId { get; set; }
    }
}
