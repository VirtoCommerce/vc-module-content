using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Data.Extensions;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Extensions;
using VirtoCommerce.SearchModule.Core.Model;
using static Npgsql.PostgresTypes.PostgresCompositeType;

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
            AddLanguage(result, file);

            AddStringFieldIfMissed(result, "OrganizationId", ContentItemConverter.Any);
            AddDateTimeFieldIfMissed(result, "StartDate", DateTime.MinValue);
            AddDateTimeFieldIfMissed(result, "EndDate", DateTime.MaxValue);

            if (result.Fields.All(x => !x.Name.EqualsIgnoreCase("UserGroups")))
            {
                result.AddFilterableCollectionAndContentString("UserGroups", [ContentItemConverter.Any]);
            }

            var folder = file.ParentUrl;
            if (!string.IsNullOrEmpty(folder))
            {
                var parts = folder.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var paths = new List<string> { "/" };
                var path = new StringBuilder();
                foreach (var part in parts)
                {
                    path.Append($"/{part}");
                    paths.Add(path.ToString());
                }

                result.AddFilterableCollection("FolderUrl", paths);
                result.AddContentString(file.ParentUrl);

            }

            return result;
        }

        private static void AddLanguage(IndexDocument result, IndexableContentFile file)
        {
            var language = file.Name.GetLanguage();

            if (language.IsNullOrEmpty())
            {
                language = ContentItemConverter.Any;
            }

            result.AddFilterableStringAndContentString("CultureName", language);
        }

        private static void RemoveFieldAndAddNew(IndexDocument document, string fieldName, string value)
        {
            var field = document.Fields.FirstOrDefault(x => x.Name.EqualsIgnoreCase(fieldName));
            if (field != null)
            {
                document.Fields.Remove(field);
            }
            document.AddFilterableStringAndContentString(fieldName, value);
        }

        private static void AddStringFieldIfMissed(IndexDocument document, string fieldName, string value)
        {
            var fieldExists = document.Fields.Any(x => x.Name.EqualsIgnoreCase(fieldName));
            if (!fieldExists)
            {
                document.AddFilterableStringAndContentString(fieldName, value);
            }
        }

        private static void AddDateTimeFieldIfMissed(IndexDocument document, string fieldName, DateTime value)
        {
            var fieldExists = document.Fields.Any(x => x.Name.EqualsIgnoreCase(fieldName));
            if (!fieldExists)
            {
                document.AddFilterableDateTime(fieldName, value);
            }
        }

        protected abstract IndexDocument BuildIndexDocumentInternal(string documentId, string storeId, IndexableContentFile file);
    }
}
