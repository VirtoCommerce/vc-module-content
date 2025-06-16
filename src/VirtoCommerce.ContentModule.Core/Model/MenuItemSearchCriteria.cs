using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Model;

public class MenuItemSearchCriteria : SearchCriteriaBase
{
    public string StoreId { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
}
