using System;
using System.Collections.Concurrent;
using VirtoCommerce.ContentModule.Core.Search;

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
                throw new InvalidOperationException($"Index Document builder is already registered for the \"{contentItemType}\" file type.");
            }
        }

        public void Override<TContentItemBuilder>(string contentItemType, Func<TContentItemBuilder> factory)
            where TContentItemBuilder : class, IContentItemBuilder
        {
            _contentItemBuilders.AddOrUpdate(contentItemType, factory, (key, oldValue) => factory);
        }
    }
}
