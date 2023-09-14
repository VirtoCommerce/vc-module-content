using VirtoCommerce.ContentModule.Core.Extensions;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public abstract class BaseContentItemBuilder : IContentItemBuilder
    {
        public virtual IndexDocument BuildIndexDocument(string storeId, string contentType, IndexableContentFile file)
        {
            var documentId = DocumentIdentifierHelper.GenerateId(storeId, file.ContentType, file);
            var result = BuildIndexDocumentInternal(documentId, storeId, file);

            result.AddFilterableStringAndContentString("ContentType", contentType);
            result.AddFilterableStringAndContentString("Name", file.Name);
            result.AddFilterableStringAndContentString("RelativeUrl", file.RelativeUrl);

            return result;
        }

        protected abstract IndexDocument BuildIndexDocumentInternal(string documentId, string storeId, IndexableContentFile file);
    }
}
