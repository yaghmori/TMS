namespace TMS.Core.Domain.Entities
{
    public interface IBaseEntity
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CreatedUserId { get; set; }
        public string? ModifiedUserId { get; set; }

        public string? CreatedIpAddress { get; set; }
        public string? ModifiedIpAddress { get; set; }

    }
}
