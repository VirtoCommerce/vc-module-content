using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Extensions;

public static class MenuSearchServiceExtensions
{
    public static async Task<bool> IsNameUnique(this IMenuSearchService searchService, string storeId, string name, string language, string id)
    {
        var menus = await searchService.SearchAllNoClone(storeId, name);
        var nameExists = menus.Any(x => x.LanguageEquals(language) && x.Id != id);

        return !nameExists;
    }

    private static bool LanguageEquals(this Menu menu, string language)
    {
        return
            string.IsNullOrEmpty(menu.Language) && string.IsNullOrEmpty(language) ||
            menu.Language.EqualsInvariant(language);
    }

    public static Task<IList<Menu>> SearchAllNoClone(this IMenuSearchService searchService, string storeId = null, string name = null, int batchSize = 50)
    {
        return searchService.SearchAll(storeId, name, clone: false, batchSize);
    }

    public static Task<IList<Menu>> SearchAll(this IMenuSearchService searchService, string storeId = null, string name = null, bool clone = true, int batchSize = 50)
    {
        var criteria = AbstractTypeFactory<MenuSearchCriteria>.TryCreateInstance();
        criteria.StoreId = storeId;
        criteria.Name = name;
        criteria.Take = batchSize;

        return searchService.SearchAllAsync(criteria, clone);
    }
}
