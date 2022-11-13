using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TMS.Core.Domain.Entities
{
    public class UserToken : IdentityUserToken<Guid>, IBaseEntity
    {
        public virtual User User { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedUserId { get; set; }
        public string? ModifiedUserId { get; set; }

        public string? CreatedIpAddress { get; set; }
        public string? ModifiedIpAddress { get; set; }

    }

}
