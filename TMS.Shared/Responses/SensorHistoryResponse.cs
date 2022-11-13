using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Shared.Responses
{
    public class SensorHistoryResponse
    {

        public Guid Id { get; set; }
        public Guid SensorId { get; set; }
        public DateTime ReadDateTime { get; set; }
        public long? ReadIndex { get; set; }
        public int? Value { get; set; }
        public int? FalseValue { get; set; }
        public int? ConvertedValue { get; set; }
    }
}
