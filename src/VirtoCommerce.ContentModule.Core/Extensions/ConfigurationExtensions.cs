using Microsoft.Extensions.Configuration;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool IsContentFullTextSearchEnabled(this IConfiguration configuration)
        {
            var value = configuration["Search:ContentFullTextSearchEnabled"];
            return value.TryParse(false);
        }
    }
}
