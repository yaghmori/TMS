using System.ComponentModel.DataAnnotations;

namespace TMS.Shared.Requests
{
    public class UserRequest
    {
        public Guid? Id { get; set; }

        public string? ProfileName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string NormalizedEmail => Email.Normalize().ToUpper();
        public string NormalizedUserName => Email.Normalize().ToUpper();
        public string UserName => Email;


    }
}
