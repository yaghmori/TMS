using System.ComponentModel.DataAnnotations;

namespace TMS.Core.Domain.Enums
{
    public enum PhoneNumberTypeEnum : int
    {

        [Display(Name = "(شخصی)موبایل", Description = "Mobile")]
        Mobile = 1,
        [Display(Name = "موبایل(کاری)", Description = "WorkMobile")]
        WorkMobile = 2,
        [Display(Name = "محل کار", Description = "Work")]
        Work = 3,
        [Display(Name = "فکس", Description = "Fax")]
        Fax = 4,
        [Display(Name = "پروژه", Description = "Project")]
        Project = 5,
        [Display(Name = "منزل", Description = "Home")]
        Home = 6,
        [Display(Name = "سایر", Description = "Other")]
        Other = 7,
    }
}
