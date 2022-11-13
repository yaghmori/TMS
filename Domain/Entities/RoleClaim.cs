using Microsoft.AspNetCore.Identity;

namespace TMS.Core.Domain.Entities
{
    public class RoleClaim : IdentityRoleClaim<Guid>, IBaseEntity
    {
        public virtual Role Role { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedUserId { get; set; }
        public string? ModifiedUserId { get; set; }


        public string? CreatedIpAddress { get; set; }
        public string? ModifiedIpAddress { get; set; }
    }

}
