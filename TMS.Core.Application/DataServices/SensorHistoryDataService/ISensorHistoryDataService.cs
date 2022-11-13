using TMS.Core.Domain.Entities;
using TMS.Shared.PagedCollections;

namespace TMS.Shared.DataServices
{
    public interface ISensorHistoryDataService
    {
        Task<IPagedList<SensorHistory>> GetHistoriesPagedAsync(int page = 0, int pageSize = 10, string query = null);
    }
}