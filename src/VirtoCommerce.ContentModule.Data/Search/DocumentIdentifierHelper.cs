using System;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Data.Search
{
    internal static class DocumentIdentifierHelper
    {
        public static string GenerateId(string storeId, ContentFile file)
        {
            return $"{storeId}::{file.RelativeUrl.Replace('/', ':')}";
        }

        public static (string storeId, string relativeUrl) ParseId(string id)
        {
            var result = id.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length == 2)
            {
                return (result[0], result[1].Replace(':', '/'));
            }

            return (null, null);
        }
    }
}
