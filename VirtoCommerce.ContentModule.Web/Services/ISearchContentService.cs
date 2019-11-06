using VirtoCommerce.ContentModule.Web.Models;

namespace VirtoCommerce.ContentModule.Web.Services
{
    public interface ISearchContentService
    {
        ContentSearchResult SearchContent(ContentSearchCriteria criteria);
    }
}
