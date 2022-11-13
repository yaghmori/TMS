using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using TMS.Core.Domain.Entities;
using TMS.Shared.Constants;
using TMS.Shared.Responses;

namespace TMS.DataAccess.Context
{
    public partial class ServerDbContext : IdentityDbContext<
        User, Role, Guid,
        UserClaim, UserRole, UserLogin,
        RoleClaim, UserToken>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public ServerDbContext(DbContextOptions<ServerDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<UserTenant> UserTenants { get; set; }
        public virtual DbSet<AppSetting> AppSettings { get; set; }
        public virtual DbSet<UserSetting> UserSettings { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<Culture> Cultures { get; set; }
        public virtual DbSet<UserSession> UserSessions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(b =>
            {
                b.ToTable("Users");
                // The relationships between User and other entity types
                // Note that these relationships are configured with no navigation properties

                // Each User can have many UserClaims
                b.HasMany<UserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

                // Each User can have many UserLogins
                b.HasMany<UserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

                // Each User can have many UserTokens
                b.HasMany<UserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

                // Each User can have many UserTokens
                b.HasMany<UserSession>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

                // Each User can have many entries in the UserRole join table
                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
                b.HasMany<UserSetting>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
                b.HasOne(b => b.Settings).WithOne(i => i.User).HasForeignKey<UserSetting>(b => b.UserId);
            });

            builder.Entity<UserClaim>(b =>
            {
                b.ToTable("UserClaims");
                b.HasOne(e => e.User).WithMany(e => e.UserClaims).HasForeignKey(e => e.UserId);

            });

            builder.Entity<UserLogin>(b =>
            {
                b.ToTable("UserLogins");
                b.HasOne(e => e.User).WithMany(e => e.UserLogins).HasForeignKey(e => e.UserId);

            });
            builder.Entity<UserSession>(b =>
            {
                b.ToTable("UserSessions");
                b.HasOne(e => e.User).WithMany(e => e.UserSessions).HasForeignKey(e => e.UserId);
            });
            builder.Entity<UserToken>(b =>
            {
                b.ToTable("UserToken");
                b.HasOne(e => e.User).WithMany(e => e.UserTokens).HasForeignKey(e => e.UserId);

            });

            builder.Entity<Role>(b =>
            {
                b.ToTable("Roles");

                // Each Role can have many entries in the UserRole join table
                b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();

                // Each Role can have many associated RoleClaims
                b.HasMany<RoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
            });

            builder.Entity<RoleClaim>(b =>
            {
                b.ToTable("RoleClaims");
                b.HasOne(e => e.Role).WithMany(e => e.RoleClaims).HasForeignKey(e => e.RoleId);

            });

            builder.Entity<UserRole>(b =>
            {
                b.ToTable("UserRoles");
                b.HasOne(e => e.User).WithMany(e => e.UserRoles).HasForeignKey(e => e.UserId);
                b.HasOne(e => e.Role).WithMany(e => e.UserRoles).HasForeignKey(e => e.RoleId);

            });





            ////SYSADMIN
            var user = new User
            {
                Id = Guid.Parse("dc206917-2b17-45ca-9929-72cc08ad2f1d"),
                SecurityStamp = Guid.NewGuid().ToString(),
                Email = "sysadmin@tms.com",
                EmailConfirmed = true,
                UserName = "sysadmin",
                ProfileName = "System Admin",
                FirstName = "sys",
                LastName = "admin",
                PhoneNumber = "",
                PhoneNumberConfirmed = true,
                NormalizedUserName = "SYSADMIN",
                NormalizedEmail = "SYSADMIN@TMS.COM",
            };
            PasswordHasher<User> ph = new PasswordHasher<User>();
            var pass = "123456789";
            user.PasswordHash = ph.HashPassword(user, pass);


            builder.Entity<User>().HasData(user);
            builder.Entity<Culture>().HasData(Seed.Cultures);
            builder.Entity<Role>().HasData(Seed.Roles);
            builder.Entity<UserRole>().HasData(new UserRole(user.Id, Seed.Roles.FirstOrDefault(x => x.Name == Shared.Constants.ApplicationRoles.SysAdmin).Id));
            builder.Entity<UserSetting>().HasData(new UserSetting
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RightToLeft = false,
                Culture = "en-US",
                DarkMode = false,
            });
            builder.Entity<AppSetting>().HasData(new AppSetting
            {
                Id = Guid.NewGuid(),
                Key = ApplicationConstants.DefaultConnectionString,
                Value = "Server=.;Database={0};Integrated Security = True;encrypt=false;",
            });
            var i = 1;

            foreach (var item in ApplicationPermissions.GetAllPermissions())
            {
                builder.Entity<RoleClaim>().HasData(new RoleClaim
                {
                    Id =i,
                    RoleId = Seed.Roles.FirstOrDefault(x => x.Name == ApplicationRoles.SysAdmin).Id,
                    ClaimType = ApplicationClaimTypes.Permission,
                    ClaimValue = item,
                }); ;
                i++;
            }
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
