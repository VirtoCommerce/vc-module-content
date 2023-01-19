using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Search;
using VirtoCommerce.ContentModule.Core.Services;
using VirtoCommerce.SearchModule.Core.Services;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class ContentItemTypeRegistrar : IContentItemTypeRegistrar
    {
        private readonly ConcurrentDictionary<string, Func<IContentItemBuilder>> _contentItemBuilders = new();

        public IContentItemBuilder GetContentItemBuilderByType(string contentItemType)
        {
            if (contentItemType == null)
            {
                throw new ArgumentNullException(nameof(contentItemType));
            }

            if (_contentItemBuilders.TryGetValue(contentItemType, out var builderFactory))
            {
                return builderFactory();
            }
            return null;
        }

        public void RegisterContentItemType<TContentItemBuilder>(string contentItemType, Func<TContentItemBuilder> factory)
             where TContentItemBuilder : class, IContentItemBuilder
        {
            if (!_contentItemBuilders.TryAdd(contentItemType, factory))
            {
                throw new InvalidOperationException($"There is already registered Index Document Builder for the \"{contentItemType}\" file type.");
            }
        }

        public void Override<TContentItemBuilder>(string contentItemType, Func<TContentItemBuilder> factory)
            where TContentItemBuilder : class, IContentItemBuilder
        {
            _contentItemBuilders.AddOrUpdate(contentItemType, factory, (key, oldValue) => factory);
        }
    }
}
