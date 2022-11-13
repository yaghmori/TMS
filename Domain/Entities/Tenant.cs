namespace TMS.Core.Domain.Entities
{
    public enum DbProviderKeys
    {
        Npgsql, SqlServer, MySql, Oracle, SqLite
    }

    public class Tenant : BaseEntity<Guid>
    {

        public Tenant()
        {

        }

        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public DateTime DateCreated { get; set; }
        public string? Name2 { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ExpireDate { get; set; }

        /// <summary>
        /// Used by AzureAd Authorization to store the AzureAd Client Issuer to map against.
        /// </summary>
        public string? Issuer { get; set; }

        public void AddValidity(int months) =>
                ExpireDate = ExpireDate.AddMonths(months);

        public void SetValidity(in DateTime validTill) =>
            ExpireDate = ExpireDate < validTill
                ? validTill
                : throw new Exception("Subscription cannot be backdated.");

        public virtual DbProviderKeys? DBProvider { get; set; } = DbProviderKeys.SqlServer;
        public string ConnectionString { get; set; } = string.Empty;
        public virtual ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>();

    }

}
