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

            if (_contentItemBuilders.TryGetValue(contentItemType.ToUpperInvariant(), out var builderFactory))
            {
                return builderFactory();
            }

            return null;
        }

        public bool IsRegisteredContentItemType(string filepath)
        {
            var extension = System.IO.Path.GetExtension(filepath);
            return _contentItemBuilders.ContainsKey(extension.ToUpperInvariant());
        }

        public void RegisterContentItemType<TContentItemBuilder>(string contentItemType, Func<TContentItemBuilder> factory)
            where TContentItemBuilder : class, IContentItemBuilder
        {
            if (!_contentItemBuilders.TryAdd(contentItemType.ToUpperInvariant(), factory))
            {
                throw new InvalidOperationException($"Index Document builder is already registered for the '{contentItemType}' file type.");
            }
        }

        public void Override<TContentItemBuilder>(string contentItemType, Func<TContentItemBuilder> factory)
            where TContentItemBuilder : class, IContentItemBuilder
        {
            _contentItemBuilders.AddOrUpdate(contentItemType.ToUpperInvariant(), factory, (key, oldValue) => factory);
        }
    }
}
