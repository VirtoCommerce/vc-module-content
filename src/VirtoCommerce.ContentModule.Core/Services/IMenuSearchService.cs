using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;
using VirtoCommerce.Platform.Core.GenericCrud;

namespace VirtoCommerce.ContentModule.Core.Services;
public interface IMenuSearchService : ISearchService<MenuSearchCriteria, MenuSearchResult, Menu>
{
}
