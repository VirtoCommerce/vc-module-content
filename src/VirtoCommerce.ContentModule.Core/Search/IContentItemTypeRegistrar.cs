using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.ContentModule.Core.Search
{
    public interface IContentItemTypeRegistrar
    {
        void RegisterContentItemType<TContentItemBuilder>(string contentItemType, Func<TContentItemBuilder> contentItemBuilder)
            where TContentItemBuilder: class, IContentItemBuilder;
        
        void Override<TContentItemBuilder>(string contentItemType, Func<TContentItemBuilder> factory)
            where TContentItemBuilder : class, IContentItemBuilder;

        IContentItemBuilder GetContentItemBuilderByType(string contentItemType);
    }
}
