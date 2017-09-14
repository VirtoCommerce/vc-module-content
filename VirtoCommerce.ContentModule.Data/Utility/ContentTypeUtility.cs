using System;

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
                contentType.StartsWith("application/j", System.StringComparison.OrdinalIgnoreCase) ;
        }
    }
}
