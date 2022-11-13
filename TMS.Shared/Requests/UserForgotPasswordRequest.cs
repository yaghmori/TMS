using System.ComponentModel.DataAnnotations;

namespace TMS.Shared.Requests
{
    public class UserForgotPasswordRequest

    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

    }
}
