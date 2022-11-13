using System.ComponentModel;

namespace TMS.Core.Domain.Enums
{
    public enum HistoryDateFilterEnum : int
    {
        [DescriptionAttribute("امروز")]
        Today = 1,
        [DescriptionAttribute("24 ساعت گذشته")]
        Last24Hours = 2,
        [DescriptionAttribute("7 روز گذشته")]
        Last7Days = 3,
        [DescriptionAttribute("30 روز گذشته")]
        Last30Days = 4,
        [DescriptionAttribute("بازه زمانی دلخواه")]
        Custom = 5,

    }

}
