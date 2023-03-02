using System;
using System.Threading.Tasks;

namespace VirtoCommerce.ContentModule.Core.Services
{
    public interface IContentStatisticService
    {
        Task<int> GetStorePagesCountAsync(string storeId);
        Task<int> GetStoreChangedPagesCountAsync(string storeId, DateTime? startDate, DateTime? endDate);
        Task<int> GetStoreThemesCountAsync(string storeId);
        Task<int> GetStoreBlogsCountAsync(string storeId);
    }
}
