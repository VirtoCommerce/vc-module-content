using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.ContentModule.Core.Model
{
    public class IndexableContentFile: ContentFile
    {
        public string StoreId { get; set; }
        public string Content { get; set; }
    }
}
