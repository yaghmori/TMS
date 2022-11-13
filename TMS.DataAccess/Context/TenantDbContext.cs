using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using TMS.Core.Domain.Entities;
using TMS.Shared.Constants;

namespace TMS.DataAccess.Context
{
    public class TenantDbContext : DbContext
    {
        private IHttpContextAccessor _httpContextAccessor;

        public TenantDbContext(DbContextOptions<TenantDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public static string ConnectionString { get; set; } = string.Empty;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(ConnectionString))
                optionsBuilder.UseSqlServer(ConnectionString);
        }
        #region DbSets
        public virtual DbSet<SiloItem> SiloItems { get; set; }
        public virtual DbSet<SensorHistory> SensorHistories { get; set; }
        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext.User?.FindFirst(x => x.Type == ApplicationClaimTypes.UserId)?.Value;
            var ip = _httpContextAccessor.HttpContext.Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            this.ChangeTracker.DetectChanges();
            var entities = this.ChangeTracker.Entries<IBaseEntity>();

            var entityEntries = entities as EntityEntry<IBaseEntity>[] ?? entities.ToArray();

            foreach (var item in entityEntries.Where(s => s.State == EntityState.Added))
            {
                item.Entity.CreatedDate = DateTime.UtcNow;
                item.Entity.CreatedIpAddress = ip;
                item.Entity.CreatedUserId = userId;
            }

            foreach (var item in entityEntries.Where(s => s.State == EntityState.Modified))
            {
                item.Entity.ModifiedDate = DateTime.UtcNow;
                item.Entity.ModifiedIpAddress = ip;
                item.Entity.ModifiedUserId = userId;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext.User?.FindFirst(x => x.Type == ApplicationClaimTypes.UserId)?.Value;
            var ip = _httpContextAccessor.HttpContext.Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            this.ChangeTracker.DetectChanges();
            var entities = this.ChangeTracker.Entries<IBaseEntity>();

            var entityEntries = entities as EntityEntry<IBaseEntity>[] ?? entities.ToArray();

            foreach (var item in entityEntries.Where(s => s.State == EntityState.Added))
            {
                item.Entity.CreatedDate = DateTime.UtcNow;
                item.Entity.CreatedIpAddress = ip;
                item.Entity.CreatedUserId = userId;
            }

            foreach (var item in entityEntries.Where(s => s.State == EntityState.Modified))
            {
                item.Entity.ModifiedDate = DateTime.UtcNow;
                item.Entity.ModifiedIpAddress = ip;
                item.Entity.ModifiedUserId = userId;
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


    }


}

