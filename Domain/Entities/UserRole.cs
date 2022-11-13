using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Core.Domain.Entities
{
    public class UserRole : IdentityUserRole<Guid>, IBaseEntity
    {
        public UserRole()
        {

        }
        public UserRole(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedUserId { get; set; }
        public string? ModifiedUserId { get; set; }

        public string? CreatedIpAddress { get; set; }
        public string? ModifiedIpAddress { get; set; }
    }

}
