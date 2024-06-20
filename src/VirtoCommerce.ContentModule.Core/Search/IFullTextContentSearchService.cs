using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.ContentModule.Core.Search
{
    public interface IFullTextContentSearchService : ISearchService<ContentSearchCriteria, ContentSearchResult, IndexableContentFile>
    {
    }
}
