using System.ComponentModel.DataAnnotations;

namespace TMS.Shared.Requests
{
    public class HomeItemRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public string? Description { get; set; } = string.Empty;
    }

}
