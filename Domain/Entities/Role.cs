using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Core.Domain.Entities
{
    public class Role : IdentityRole<Guid>, IBaseEntity
    {

        public Role()
        {
        }

        public Role(string roleName) : base(roleName)
        {
            NormalizedName = roleName.Normalize().ToUpper();
        }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();
        //public virtual ICollection<User> Users { get; set; } = new List<User>();
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedUserId { get; set; }
        public string? ModifiedUserId { get; set; }

        public string? CreatedIpAddress { get; set; }
        public string? ModifiedIpAddress { get; set; }


    }

}
