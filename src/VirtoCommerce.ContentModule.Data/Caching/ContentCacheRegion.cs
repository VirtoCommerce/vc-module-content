using System;
using Microsoft.Extensions.Primitives;
using VirtoCommerce.Platform.Core.Caching;

namespace VirtoCommerce.ContentModule.Data.Model
{
    public class ContentCacheRegion : CancellableCacheRegion<ContentCacheRegion>
    {
        public static IChangeToken CreateChangeToken(string contentStore)
        {
            if (contentStore == null)
            {
                throw new ArgumentNullException(nameof(contentStore));
            }
            return CreateChangeTokenForKey(contentStore);
        }

        public static void ExpireContent(string contentStore)
        {
            if (contentStore != null)
            {
                ExpireTokenForKey(contentStore);
            }
        }
    }
}
