using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TMS.Core.Domain.Entities;
using TMS.Shared.PagedCollections;

namespace TMS.Infrastructure.Infrastructure.RepositoriesInterfaces
{
    public interface ISiloItemRepository : IRepository<SiloItem>
    {
        IEnumerable<SiloItem> GetChildFlatten(Guid id);
        Task<IPagedList<TResult>> GetPagedSensorHistoryAsync<TResult>(Guid sensorId,
            Expression<Func<SensorHistory, TResult>> selector,
            string? searchQuery = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            Expression<Func<SensorHistory, bool>> predicate = null,
            Func<IQueryable<SensorHistory>, IOrderedQueryable<SensorHistory>> orderBy = null,
            Func<IQueryable<SensorHistory>, IIncludableQueryable<SensorHistory, object>> include = null,
            int pageIndex = 0,
            int pageSize = 20,
            bool disableTracking = true,
            CancellationToken cancellationToken = default,
            bool ignoreQueryFilters = false) where TResult : class;
    }
}
