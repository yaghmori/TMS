using MudBlazor;
using TMS.Core.Domain.Entities;
using TMS.Core.Domain.Enums;

namespace TMS.Shared.Responses
{
    public class SiloItemResponse
    {

        public string Id { get; set; }
        public virtual SiloItemTypeEnum ItemType { get; set; }
        public int Index { get; set; }
        public int? Address { get; set; }
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
        public SiloItemResponse? Parent { get; set; }
        public string? ParentId { get; set; }
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
        public virtual ICollection<SensorHistory> Histories { get; set; }
        public virtual List<SiloItemResponse> SiloItems { get; set; } = new();
        public virtual List<SiloItemResponse> FeaturedSensors { get; set; } = new();
        public virtual List<SiloItemResponse> ParentList { get; set; } = new();

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
        public string Icon
        {
            get
            {
                switch (ItemType)
                {
                    case SiloItemTypeEnum.Silo:
                        return "fad fa-database";
                    case SiloItemTypeEnum.Loop:
                        return "fad fa-ring";
                    case SiloItemTypeEnum.Cable:
                        return "fad fa-diagram-nested";
                    case SiloItemTypeEnum.TempSensor:
                        if (Feature == SensorFeatureEnum.AmbientTemperature)
                            return "fad fa-temperature-sun";
                        else
                            return "fad fa-microchip";
                    case SiloItemTypeEnum.HumiditySensor:
                        if (Feature == SensorFeatureEnum.AmbientHumidity)
                            return "fad fa-droplet-percent";
                        else
                            return "fad fa-droplet-percent";
                    default:
                        return "fad fa-microchip";
                }
            }
        }
        public string Color => Colors.Red.Default;

        public bool HasChild => SiloItems.Count > 0;

    }
}
