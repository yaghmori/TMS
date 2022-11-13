using System.ComponentModel.DataAnnotations;

namespace TMS.Core.Domain.Enums
{
    public enum EmailTypeEnum : int
    {
        [Display(Name = "شخصی", Description = "Personal")]
        Personal = 1,
        [Display(Name = "کاری", Description = "Work")]
        Work = 2,
        [Display(Name = "پروژه", Description = "Project")]
        Project = 3,
        [Display(Name = "سایر", Description = "Other")]
        Other = 4,
    }
}
