using VirtoCommerce.ContentModule.Data.Search;
using VirtoCommerce.ContentModule.Web.Models;

namespace VirtoCommerce.ContentModule.Web.Services
{
    public interface IContentSearchService
    {
        ContentSearchResult SearchContent(ContentSearchCriteria criteria);
    }
}
