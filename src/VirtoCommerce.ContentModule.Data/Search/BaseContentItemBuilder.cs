using System.Linq;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public abstract class BaseContentItemBuilder : IContentItemBuilder
    {
        public virtual IndexDocument BuildIndexDocument(string storeId, string contentType, IndexableContentFile file)
        {
            var documentId = DocumentIdentifierHelper.GenerateId(storeId, file.ContentType, file);
            var result = BuildIndexDocumentInternal(documentId, storeId, file);

            RemoveFieldAndAddNew(result, "ContentType", contentType);
            RemoveFieldAndAddNew(result, "Name", file.Name);
            RemoveFieldAndAddNew(result, "RelativeUrl", file.RelativeUrl);
            result.AddSuggestableString("FolderUrl", file.ParentUrl);
            return result;
        }

        private static void RemoveFieldAndAddNew(IndexDocument document, string fieldName, string value)
        {
            var field = document.Fields.FirstOrDefault(x => x.Name.EqualsInvariant(fieldName));
            if (field != null)
            {
                document.Fields.Remove(field);
            }
            document.AddFilterableStringAndContentString(fieldName, value);
        }

        protected abstract IndexDocument BuildIndexDocumentInternal(string documentId, string storeId, IndexableContentFile file);
    }
}
