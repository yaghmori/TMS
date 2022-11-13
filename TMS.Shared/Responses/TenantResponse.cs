using System.ComponentModel.DataAnnotations;
using TMS.Core.Domain.Entities;

namespace TMS.Shared.Responses
{
    public class TenantResponse
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public virtual DbProviderKeys? DBProvider { get; set; }
        public string? ConnectionString { get; set; }=string.Empty;
        public DateTime? ExpireDate { get; set; }
        public List<UserResponse> Users { get; set; } = new();
        public int UsersCount => Users.Count;
        public bool Accessable => ExpireDate > DateTime.UtcNow && IsActive;
        public bool NormalizedName { get; set; }

    }

}
