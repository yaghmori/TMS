using Microsoft.AspNetCore.Components;
using TMS.Shared.Models;

namespace TMS.RootComponents.Shared.Components
{
    public partial class BreadCrumbComponent
    {
        [Parameter]
        public List<MyBreadcrumbItem> Links { get; set; }

        public BreadCrumbComponent()
        {
        }

        protected override Task OnParametersSetAsync() => base.OnParametersSetAsync();
    }
}