using Microsoft.AspNetCore.Components;
using TMS.Shared.Requests;

namespace TMS.RootComponents.Shared.Components
{
    public partial class SiloItemPathComponent
    {
        [Parameter]
        public List<SiloItemRequest> Items { get; set; }
        [Parameter]
        public string Class { get; set; }

        public SiloItemPathComponent()
        {
        }

        protected override Task OnParametersSetAsync() => base.OnParametersSetAsync();
    }
}