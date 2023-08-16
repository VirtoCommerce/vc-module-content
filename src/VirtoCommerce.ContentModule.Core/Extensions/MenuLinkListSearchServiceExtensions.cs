using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Extensions;

public static class MenuLinkListSearchServiceExtensions
{
    public static async Task<bool> IsNameUnique(this IMenuLinkListSearchService searchService, string storeId, string name, string language, string id)
    {
        var lists = await searchService.SearchAllNoClone(storeId, name);
        var nameExists = lists.Any(x => x.LanguageEquals(language) && x.Id != id);

        return !nameExists;
    }

    private static bool LanguageEquals(this MenuLinkList menuLinkList, string language)
    {
        return
            string.IsNullOrEmpty(menuLinkList.Language) && string.IsNullOrEmpty(language) ||
            menuLinkList.Language.EqualsInvariant(language);
    }

    public static Task<IList<MenuLinkList>> SearchAllNoClone(this IMenuLinkListSearchService searchService, string storeId = null, string name = null, int batchSize = 50)
    {
        return searchService.SearchAll(storeId, name, clone: false, batchSize);
    }

    public static Task<IList<MenuLinkList>> SearchAll(this IMenuLinkListSearchService searchService, string storeId = null, string name = null, bool clone = true, int batchSize = 50)
    {
        var criteria = AbstractTypeFactory<MenuLinkListSearchCriteria>.TryCreateInstance();
        criteria.StoreId = storeId;
        criteria.Name = name;
        criteria.Take = batchSize;

        return searchService.SearchAllAsync(criteria, clone);
    }
}
