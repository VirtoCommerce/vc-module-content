using System;

namespace VirtoCommerce.ContentModule.Core.Search
{
    public interface IContentItemTypeRegistrar
    {
        void RegisterContentItemType<TContentItemBuilder>(string contentItemType, Func<TContentItemBuilder> contentItemBuilder)
            where TContentItemBuilder : class, IContentItemBuilder;

        void Override<TContentItemBuilder>(string contentItemType, Func<TContentItemBuilder> factory)
            where TContentItemBuilder : class, IContentItemBuilder;

        IContentItemBuilder GetContentItemBuilderByType(string contentItemType);
        bool IsRegisteredContentItemType(string filepath);
    }
}
