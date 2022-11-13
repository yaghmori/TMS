
namespace TMS.Core.Domain.Entities
{
    public class SensorHistory : BaseEntity<Guid>
    {
        public Guid SensorId { get; set; }
        public virtual SiloItem Sensor { get; set; }
        public DateTime ReadDateTime { get; set; }
        public int? Value { get; set; }
        public int? FalseValue { get; set; }
        public long? ReadIndex { get; set; }


    }

}
