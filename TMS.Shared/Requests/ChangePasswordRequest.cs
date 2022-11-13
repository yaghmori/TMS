using System.ComponentModel.DataAnnotations;

namespace TMS.Shared.Requests
{
    public class ChangePasswordRequest

    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = default!;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        public string NewPassword { get; set; } = default!;

        [Required]
        [Compare(nameof(NewPassword))]
        [DataType(DataType.Password)]
        [StringLength(30, ErrorMessage = "Confirmation Password must be at least 8 characters long.", MinimumLength = 8)]
        public string ConfirmationPassword { get; set; } = default!;

    }
}
