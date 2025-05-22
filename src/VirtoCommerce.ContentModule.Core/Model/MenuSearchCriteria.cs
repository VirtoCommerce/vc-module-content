using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model;

public class MenuSearchCriteria : SearchCriteriaBase
{
    public string StoreId { get; set; }
    public string Name { get; set; }
}
