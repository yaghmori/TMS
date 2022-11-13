using System.ComponentModel.DataAnnotations;

namespace TMS.Shared.Requests
{
    public class UserClientRequest
    {

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid ClientId { get; set; }
    }

}
