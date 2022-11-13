using System.ComponentModel.DataAnnotations;

namespace TMS.Core.Domain.Enums
{
    public enum AddressTypeEnum : int
    {

        [Display(Name = "منزل", Description = "Home")]
        Home = 1,
        [Display(Name = "محل کار", Description = "Work")]
        Work = 2,
        [Display(Name = "پروژه", Description = "Project")]
        Project = 3,
        [Display(Name = "سایر", Description = "Other")]
        Others = 4,
    }
}
