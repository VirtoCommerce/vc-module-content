using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class ContentPathResolver : IContentPathResolver
    {
        private readonly ContentOptions _options;

        public ContentPathResolver(IOptions<ContentOptions> options)
        {
            _options = options.Value;
        }

        public string GetContentBasePath(string contentType, string storeId, string themeName)
        {
            return GetContentPathFromMappings(contentType, storeId, themeName)
                ?? GetDefaultContentPath(contentType, storeId, themeName);
        }

        private string GetContentPathFromMappings(string contentType, string storeId, string themeName)
        {
            if (_options.PathMappings != null && _options.PathMappings.Any() && _options.PathMappings.ContainsKey(contentType))
            {
                var theme = themeName ?? ContentConstants.DefaultTheme;
                var mapping = _options.PathMappings[contentType];
                var parts = mapping.Select(x => x switch
                {
                    "_storeId" => storeId,
                    "_theme" => themeName,
                    "_blog" => ContentConstants.ContentTypes.Blogs,
                    _ => x,
                });
                var result = string.Join('/', parts);
                return result;
            }

            return null;
        }

        private static string GetDefaultContentPath(string contentType, string storeId, string themeName)
        {
            var retVal = contentType switch
            {
                var x when x.EqualsInvariant(ContentConstants.ContentTypes.Themes) => "Themes/" + storeId,
                var x when x.EqualsInvariant(ContentConstants.ContentTypes.Pages) => "Pages/" + storeId,
                var x when x.EqualsInvariant(ContentConstants.ContentTypes.Blogs) => $"Pages/{storeId}/{ContentConstants.ContentTypes.Blogs}",
                var _ => string.Empty
            };

            return retVal;
        }
    }
}
