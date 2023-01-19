using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Search
{
    public interface IContentItemBuilder
    {
        IndexDocument BuildIndexDocument(string storeId, IndexableContentFile file);
    }
}
