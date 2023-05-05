using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public abstract class BaseContentItemBuilder : IContentItemBuilder
    {
        public virtual IndexDocument BuildIndexDocument(string storeId, IndexableContentFile file)
        {
            var documentId = DocumentIdentifierHelper.GenerateId(storeId, file.ContentType, file);
            return BuildIndexDocumentInternal(documentId, storeId, file);
        }

        protected abstract IndexDocument BuildIndexDocumentInternal(string documentId, string storeId, IndexableContentFile file);
    }
}
