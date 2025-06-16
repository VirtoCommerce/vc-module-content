using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Extensions;

public static class MenuItemSearchServiceExtensions
{
    public static async Task<bool> IsNameUnique(this IMenuItemSearchService searchService, string storeId, string name, string language, string id)
    {
        var menus = await searchService.SearchAllNoClone(storeId, name);
        var nameExists = menus.Any(x => x.LanguageEquals(language) && x.Id != id);

        return !nameExists;
    }

    private static bool LanguageEquals(this MenuItem menu, string language)
    {
        return
            string.IsNullOrEmpty(menu.LanguageCode) && string.IsNullOrEmpty(language) ||
            menu.LanguageCode.EqualsInvariant(language);
    }

    public static Task<IList<MenuItem>> SearchAllNoClone(this IMenuItemSearchService searchService, string storeId = null, string type = null, string name = null, int batchSize = 50)
    {
        return searchService.SearchAll(storeId, type, name, clone: false, batchSize);
    }

    public static Task<IList<MenuItem>> SearchAll(this IMenuItemSearchService searchService, string storeId = null, string type = null, string name = null, bool clone = true, int batchSize = 50)
    {
        var criteria = AbstractTypeFactory<MenuItemSearchCriteria>.TryCreateInstance();
        criteria.StoreId = storeId;
        criteria.Type = type;
        criteria.Name = name;
        criteria.Take = batchSize;

        return searchService.SearchAllAsync(criteria, clone);
    }
}
