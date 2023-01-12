using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IContentPathResolver
    {
        string GetContentBasePath(string contentType, string storeId, string themeName = null);
    }
}
