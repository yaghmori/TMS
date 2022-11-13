using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Core.Domain.Entities
{
    public class User : IdentityUser<Guid>, IBaseEntity
    {
        public User()
        {

        }
        public string? ProfileName { get; set; } = string.Empty;
        public string? FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailTokenExpires { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public string? PhoneNumberVerificationToken { get; set; }
        public DateTime? PhoneNumberTokenExpires { get; set; }
        public DateTime? PhoneNumberVerifiedAt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordTokenExpires { get; set; }
        public DateTime? PasswordResetedAt { get; set; }
        public bool IsActive { get; set; } = true;
        public  Guid SettingsId { get; set; }
        public virtual UserSetting Settings { get; set; }
        public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
        public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
        public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();
        public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<Person> Persons { get; set; } = new List<Person>();
        public virtual ICollection<UserTenant> UserTenants { get; set; } = new List<UserTenant>();
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedUserId { get; set; }
        public string? ModifiedUserId { get; set; }
        public string? CreatedIpAddress { get; set; }
        public string? ModifiedIpAddress { get; set; }
    }

}
