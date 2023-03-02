using System;
using System.Text;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Data.Search
{
    internal static class DocumentIdentifierHelper
    {
        public static string GenerateId(string storeId, ContentFile file)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes($"{storeId}::{file.RelativeUrl}")).Replace('=', '-');
        }

        public static (string storeId, string relativeUrl) ParseId(string id)
        {
            var decoded = Encoding.ASCII.GetString(Convert.FromBase64String(id.Replace('-', '=')));
            var result = decoded.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

            if (result.Length == 2)
            {
                return (result[0], result[1]);
            }

            return (null, null);
        }
    }
}
