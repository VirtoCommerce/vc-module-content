using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Search
{
    public interface IContentItemBuilder
    {
        IndexDocument BuildIndexDocument(string storeId, IndexableContentFile file);
    }
}
