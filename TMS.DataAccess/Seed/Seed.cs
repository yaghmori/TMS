using TMS.Core.Domain.Entities;

namespace TMS.DataAccess
{
    internal class Seed
    {


        internal static List<Role> Roles = new List<Role> {
            new Role { Id=new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2"), Name = Shared.Constants.ApplicationRoles.SysAdmin, NormalizedName = Shared.Constants.ApplicationRoles.SysAdmin.Normalize().ToUpper() },
            new Role { Id=new Guid("b9afe837-7566-4516-93fa-cdc3d0f9289d"), Name = Shared.Constants.ApplicationRoles.Admin, NormalizedName = Shared.Constants.ApplicationRoles.Admin.Normalize().ToUpper() },
            new Role { Id=new Guid("c3c1384e-9822-49f1-97c5-0d065426329b"), Name = Shared.Constants.ApplicationRoles.User, NormalizedName = Shared.Constants.ApplicationRoles.User.Normalize().ToUpper() },
            new Role { Id=new Guid("fe299f1c-4a8c-4f2b-8c9e-52091391781e"), Name = Shared.Constants.ApplicationRoles.Owner, NormalizedName = Shared.Constants.ApplicationRoles.Owner.Normalize().ToUpper() },
        };


        internal static List<Culture> Cultures = new List<Culture> {
            new Culture {Id= Guid.NewGuid(),CultureName= "en-US", NormalizedCultureName="en-US".Normalize().ToUpper(),  DisplayName="English",NormalizedDisplayName ="English".Normalize().ToUpper(),ShortDatePattern="yyyy/MM/dd",LongDatePattern="dddd, MMMM dd, yyyy",LongTimePattern="HH:mm:ss",ShortTimePattern="HH:mm",FullDateTimePattern="dddd, MMMM dd, yyyy h:mm:ss tt",DateSeparator="/",TimeSeparator=":",YearMonthPattern="MMMM, yyyy",MonthDayPattern="MMMM dd",FirstDayOfWeek=DayOfWeek.Monday,Image="_content/TMS.RootComponents/assets/media/flags/usa.svg",IsDefault=true,RightToLeft=false },
            new Culture {Id= Guid.NewGuid(),CultureName= "fa-IR",NormalizedCultureName="fa-IR".Normalize().ToUpper(),DisplayName="فارسی",NormalizedDisplayName ="فارسی".Normalize().ToUpper(),ShortDatePattern="yyyy/MM/dd",LongDatePattern="dddd, MMMM dd, yyyy ",LongTimePattern="HH:mm:ss",ShortTimePattern="HH:mm",FullDateTimePattern="dddd, MMMM dd, yyyy h:mm:ss tt",DateSeparator="/",TimeSeparator=":",YearMonthPattern="MMMM, yyyy",MonthDayPattern="MMMM dd",FirstDayOfWeek=DayOfWeek.Saturday,Image="_content/TMS.RootComponents/assets/media/flags/iran.svg",IsDefault=false,RightToLeft=true }
        };
    }
}
