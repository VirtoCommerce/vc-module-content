using System;
using System.Text;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Data.Search
{
    internal static class DocumentIdentifierHelper
    {
        public static string GenerateId(string storeId, string contentType, ContentItem file)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes($"{storeId}::{contentType}::{file.RelativeUrl}")).Replace('=', '-');
        }

        public static (string storeId, string contentType, string relativeUrl) ParseId(string id)
        {
            var decoded = Encoding.ASCII.GetString(Convert.FromBase64String(id.Replace('-', '=')));
            var result = decoded.Split(["::"], StringSplitOptions.RemoveEmptyEntries);

            if (result.Length == 3)
            {
                return (result[0], result[1], result[2]);
            }

            return (null, null, null);
        }
    }
}
