using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.ContentModule.Core.Model;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IContentStatisticService
    {
        Task<ContentStatistic> GetStoreContentStatsAsync(string storeId);
    }
}
