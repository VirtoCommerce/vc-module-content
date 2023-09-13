using VirtoCommerce.SearchModule.Core.Extenstions;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Extensions
{
    public static class SearchExtensions
    {
        public const string SchemaStringValue = "schema";

        public static void AddFilterableStringAndContentString(this IndexDocument document, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                AddFilterableString(document, name, value);
                AddContentString(document, value);
            }
        }

        public static void AddFilterableString(this IndexDocument schema, string name)
        {
            AddFilterableString(schema, name, SchemaStringValue);
        }

        public static void AddFilterableString(this IndexDocument document, string name, string value)
        {
            document.AddFilterableValue(name, value, IndexDocumentFieldValueType.String);
        }

        public static void AddContentString(this IndexDocument document, string value)
        {
            AddSearchableCollection(document, IndexDocumentExtensions.SearchableFieldName, value);
        }

        public static void AddSearchableCollection(this IndexDocument document, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                document.Add(new IndexDocumentField(name, value)
                {
                    IsRetrievable = true,
                    IsSearchable = true,
                    IsCollection = true,
                    ValueType = IndexDocumentFieldValueType.String
                });
            }
        }
    }
}
