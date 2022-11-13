
using System.ComponentModel.DataAnnotations.Schema;
using TMS.Core.Domain.Enums;

namespace TMS.Core.Domain.Entities;




public class SiloItem : BaseEntity<Guid>
{

    public virtual SiloItemTypeEnum ItemType { get; set; }
    public int Index { get; set; }
    public int Address { get; set; }
    public string? Rom { get; set; }
    public string? TechNo { get; set; }
    public int? Box { get; set; }
    public int? Line { get; set; }
    public string? Name { get; set; }
    public decimal? Length { get; set; }
    public decimal? SensorSpace { get; set; }
    public string? Description { get; set; }
    public decimal? SiloDiameter { get; set; }
    public decimal? LoopDiameter { get; set; }
    public decimal? SiloHeight { get; set; }
    public Guid? ParentId { get; set; }
    public virtual SiloItem Parent { get; set; }
    public int? HighTemp { get; set; }
    public int? LowTemp { get; set; }
    public int? ChildLimit { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsExpanded { get; set; } = false;
    public bool IsReadOnly { get; set; } = false;
    public int? Value { get; set; }
    public int? FalseValue { get; set; }
    public int FalseValueCount { get; set; } = 0;
    public bool IsMapped { get; set; } = false;
    public long? ReadIndex { get; set; }
    public DateTime? ReadDateTime { get; set; }
    public int Offset { get; set; } = 0;
    public virtual WarningServiceTypeEnum WarningServiceType { get; set; }
    public virtual AirConditionEnum AirCondition { get; set; }
    public virtual SensorFeatureEnum Feature { get; set; }
    public virtual ICollection<SiloItem> SiloItems { get; set; }
    public virtual ICollection<SensorHistory> SensorHistories { get; set; }

    [NotMapped]
    public string DisplayMember
    {
        get
        {
            var name = ItemType == SiloItemTypeEnum.TempSensor ? "Sensor " + Index : ItemType == SiloItemTypeEnum.HumiditySensor ? "Humidity" : ItemType + " " + Index;
            var displayName = string.IsNullOrWhiteSpace(Name) ? "" : " (" + Name + ")";
            if (Feature != SensorFeatureEnum.None)
                name = Feature == SensorFeatureEnum.AmbientTemperature ? "Ambient Temperature Sensor" : Feature == SensorFeatureEnum.AmbientHumidity ? "Ambient Humidity Sensor" : "Not Defined";
            return name + displayName;
        }
    }

}

