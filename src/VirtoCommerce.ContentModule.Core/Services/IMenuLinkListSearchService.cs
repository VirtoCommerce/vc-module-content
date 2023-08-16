using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.ContentModule.Core.Services;

public interface IMenuLinkListSearchService : ISearchService<MenuLinkListSearchCriteria, MenuLinkListSearchResult, MenuLinkList>
{
}
