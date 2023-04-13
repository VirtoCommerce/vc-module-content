using System.Linq;
using Microsoft.Extensions.Options;
using VirtoCommerce.ContentModule.Core;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.GenericCrud;
using VirtoCommerce.StoreModule.Core.Model;
using VirtoCommerce.StoreModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Services
{
    public class ContentPathResolver : IContentPathResolver
    {
        private readonly ContentOptions _options;
        private readonly ICrudService<Store> _storeService;

        public ContentPathResolver(IOptions<ContentOptions> options, IStoreService storeService)
        {
            _options = options.Value;
            _storeService = (ICrudService<Store>)storeService;
        }

        public string GetContentBasePath(string contentType, string storeId, string themeName)
        {
            return GetContentPathFromMappings(contentType, storeId, themeName)
                ?? GetDefaultContentPath(contentType, storeId);
        }

        private string GetContentPathFromMappings(string contentType, string storeId, string themeName)
        {
            if (_options.PathMappings.IsNullOrEmpty() || !_options.PathMappings.ContainsKey(contentType))
            {
                return null;
            }

            var mapping = _options.PathMappings[contentType];

            var parts = mapping.Select(x => x switch
            {
                "_storeId" => storeId,
                "_theme" => GetThemeName(storeId, themeName),
                "_blog" => ContentConstants.ContentTypes.Blogs,
                _ => x,
            });

            return string.Join('/', parts) + "/";
        }

        private static string GetDefaultContentPath(string contentType, string storeId)
        {
            var retVal = contentType switch
            {
                _ when contentType.EqualsInvariant(ContentConstants.ContentTypes.Themes) => "Themes/" + storeId,
                _ when contentType.EqualsInvariant(ContentConstants.ContentTypes.Pages) => "Pages/" + storeId,
                _ when contentType.EqualsInvariant(ContentConstants.ContentTypes.Blogs) => $"Pages/{storeId}/{ContentConstants.ContentTypes.Blogs}",
                _ => string.Empty
            };

            return retVal + "/";
        }

        private string GetThemeName(string storeId, string themeName)
        {
            if (!string.IsNullOrEmpty(themeName))
            {
                return themeName;
            }
            var store = _storeService.GetByIdAsync(storeId).Result;
            return store?.DynamicProperties.FirstOrDefault(x => x.Name == "DefaultThemeName")?.Values?.FirstOrDefault()?.Value?.ToString() ?? ContentConstants.DefaultTheme;
        }
    }
}
