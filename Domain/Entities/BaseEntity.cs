using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TMS.Core.Domain.Entities
{
    public class BaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedUserId { get; set; } 
        public string? ModifiedUserId { get; set; }
        public string? CreatedIpAddress { get; set; }
        public string? ModifiedIpAddress { get; set; }

        public bool IsEqual(TKey a, TKey b)
        {
            return a.Equals(b);
        }


    }
}
