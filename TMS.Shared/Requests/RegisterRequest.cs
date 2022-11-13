using System.ComponentModel.DataAnnotations;

namespace TMS.Shared.Requests
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare(nameof(Password))]
        public string PasswordConfirmation { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string NormalizedEmail => Email.Normalize().ToUpper();

    }

}
