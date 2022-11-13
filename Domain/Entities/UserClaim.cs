using Microsoft.AspNetCore.Identity;

namespace TMS.Core.Domain.Entities
{
    public class UserClaim : IdentityUserClaim<Guid>, IBaseEntity
    {
        public UserClaim()
        {

        }
        public UserClaim(Guid userId, string claimType, string claimValue)
        {
            UserId = userId;
            ClaimType = claimType;
            ClaimValue = claimValue;

        }
        public virtual User User { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedUserId { get; set; }
        public string? ModifiedUserId { get; set; }
        public string? CreatedIpAddress { get; set; }
        public string? ModifiedIpAddress { get; set; }


    }

}
