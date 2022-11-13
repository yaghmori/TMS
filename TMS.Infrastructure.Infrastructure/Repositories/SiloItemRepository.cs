using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TMS.Core.Domain.Entities;
using TMS.Core.Domain.Enums;
using TMS.DataAccess.Extensions;
using TMS.Infrastructure.Infrastructure.RepositoriesInterfaces;
using TMS.Shared.PagedCollections;

namespace TMS.Infrastructure.Infrastructure.Repositories
{
    public class SiloItemRepository : Repository<SiloItem>, ISiloItemRepository
    {

        public SiloItemRepository(DbContext dbContext) : base(dbContext)
        {

        }

        public IEnumerable<SiloItem> GetChildFlatten(Guid id)
        {
            try
            {
                return _dbSet.OrderBy(o => o.Index).Where(x => x.Id == id).Include(x => x.SiloItems).GetChildFlatten(x => x.SiloItems);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<TResult>> GetAllFlatten<TResult>(Expression<Func<SiloItem, TResult>> selector,
                                                                    Expression<Func<SiloItem, bool>> predicate = null,
                                                                    Func<IQueryable<SiloItem>, IOrderedQueryable<SiloItem>> orderBy = null,
                                                                    Func<IQueryable<SiloItem>, IIncludableQueryable<SiloItem, object>> include = null,
                                                                    bool disableTracking = true,
                                                                    CancellationToken cancellationToken = default,
                                                                    bool ignoreQueryFilters = false) where TResult : class
        {
            IQueryable<SiloItem> query = _dbSet;

            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
                query = orderBy(query);

            var a = await query.Select(selector).ToListAsync(cancellationToken);
            return a;
        }



        public async Task<IPagedList<TResult>> GetPagedSensorHistoryAsync<TResult>(Guid sensorId,
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
            bool ignoreQueryFilters = false) where TResult : class
        {
            var siloItem = await _dbSet.FindAsync(sensorId);

            IQueryable<SensorHistory> query = _dbContext.Set<SensorHistory>().OrderBy(o => o.ReadDateTime);
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }

            if (siloItem.ItemType == SiloItemTypeEnum.Silo)
                query = query.Where(x => x.Sensor.Parent.Parent.ParentId == siloItem.Id);

            if (siloItem.ItemType == SiloItemTypeEnum.Loop)
                query = query.Where(x => x.Sensor.Parent.ParentId == siloItem.Id);

            if (siloItem.ItemType == SiloItemTypeEnum.Cable)
                query = query.Where(x => x.Sensor.ParentId == siloItem.Id);

            if (siloItem.ItemType == SiloItemTypeEnum.TempSensor)
                query = query.Where(x => x.SensorId == siloItem.Id);

            if (siloItem.ItemType == SiloItemTypeEnum.HumiditySensor)
                query = query.Where(x => x.SensorId == siloItem.Id);

            if (startDate != null)
                query = query.Where(x => x.ReadDateTime >= startDate);

            if (endDate != null)
                query = query.Where(x => x.ReadDateTime <= endDate);

            if (string.IsNullOrWhiteSpace(searchQuery))
                query = query.Where(x => x.Value.ToString().Contains(searchQuery)
                || x.FalseValue.ToString().Contains(searchQuery)
                || (x.Value + x.Sensor.Offset).ToString().Contains(searchQuery)
                || x.ReadIndex.ToString().Contains(searchQuery));

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (orderBy != null)
                query = orderBy(query);

            var groupQuery = query.GroupBy(g => new { g.ReadDateTime });

            var result = groupQuery.Select(s => new SensorHistory
            {
                ReadDateTime = s.Key.ReadDateTime,
                ReadIndex = s.FirstOrDefault().ReadIndex,
                Id = s.FirstOrDefault().Id,
                Value = (int?)s.Average(a => (int?)a.Value)
            }).AsQueryable();

            return await result.Select(selector).ToPagedListAsync(pageIndex, pageSize, 0, cancellationToken);
        }



    }
}
