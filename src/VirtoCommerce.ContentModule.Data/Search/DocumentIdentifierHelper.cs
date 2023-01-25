using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Model;

namespace VirtoCommerce.ContentModule.Data.Search
{
    // todo: should it be a service?
    internal static class DocumentIdentifierHelper
    {
        public static string GenerateId(string storeId, ContentFile file)
        {
            return $"{storeId}::{file.RelativeUrl}";
        }

        public static (string StoreId, string RelativeUrl) ParseId(string id)
        {
            var result = id.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
            if (result.Length == 2)
            {
                return (result[0], result[1]);
            }

            return (null, null);
        }
    }
}
