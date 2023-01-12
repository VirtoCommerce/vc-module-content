using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.ContentModule.Core.Services.Indexing;

namespace VirtoCommerce.ContentModule.Data.Search
{
    public class FullTextContentSearchService : IFullTextContentSearchService
    {
        public Task<ContentSearchResult> SearchContentAsync(ContentSearchCriteria criteria)
        {
            throw new NotImplementedException();
        }
    }
}
