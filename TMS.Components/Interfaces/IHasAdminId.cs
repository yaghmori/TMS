using Microsoft.AspNetCore.Components;

namespace TMS.RootComponents.Interfaces
{
    internal interface IHasAdminId
    {
       [Parameter] public string AdminId { get; set; }
    }
}
