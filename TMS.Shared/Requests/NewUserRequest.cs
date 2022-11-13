using System.ComponentModel.DataAnnotations;

namespace TMS.Shared.Requests
{
    public class NewUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        public string Password { get; set; } = default!;

        [Required]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "Password Confirmation must be at least 8 characters long.", MinimumLength = 8)]
        public string PasswordConfirmation { get; set; } = default!;

        public string? ProfileName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Image { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool RightToLeft { get; set; } = false;
        public bool DarkMode { get; set; } = false;
        public string Culture { get; set; } = "en-US";
        public string NormalizedEmail => Email.Normalize().ToUpper();
        public string UserName => Email;
        public string NormalizedUserName => Email.Normalize().ToUpper();

    }
}
