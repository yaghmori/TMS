using System.ComponentModel.DataAnnotations;

namespace TMS.Shared.Requests
{
    public class CategoryRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public Guid? ParentId { get; set; } = null;

    }

}
