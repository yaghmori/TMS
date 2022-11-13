using System.ComponentModel.DataAnnotations;

namespace TMS.Shared.Requests
{
    public class RoleRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
