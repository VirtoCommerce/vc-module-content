using System;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Utility
{
    public static class ContentTypeUtility
    {
        public static bool IsImageContentType(string contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));

            return contentType.StartsWith("image/", System.StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsTextContentType(string contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException(nameof(contentType));

            return contentType.StartsWith("text/", System.StringComparison.OrdinalIgnoreCase) ||
                contentType.StartsWith("application/j", System.StringComparison.OrdinalIgnoreCase);
        }

        public static string GetContentBasePath(string contentType, string storeId)
        {
            var retVal = string.Empty;
            if (contentType.EqualsInvariant("themes"))
            {
                retVal = "Themes/" + storeId;
            }
            else if (contentType.EqualsInvariant("pages"))
            {
                retVal = "Pages/" + storeId;
            }
            else if (contentType.EqualsInvariant("blogs"))
            {
                retVal = "Pages/" + storeId + "/blogs";
            }
            return retVal;
        }
    }
}
