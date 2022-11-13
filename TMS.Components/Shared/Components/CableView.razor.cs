using Microsoft.AspNetCore.Components;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Shared.Components
{
    public partial class CableView 
    {
        [Parameter]
        public string SiloItemId { get; set; } = string.Empty;
        [Parameter]
        public SiloItemResponse Silo { get; set; } = new();

        public List<SiloItemResponse> SiloItemCollection { get; set; } = new();


        protected async override Task OnParametersSetAsync()
        {
            //Silo = (await _siloItemDataService.GetByItemTypeAsync(SiloItemTypeEnum.Silo)).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(SiloItemId))
            {
                IsLoading = true;
                Silo = await _siloItemDataService.GetSiloItemByIdAsync(SiloItemId);
                IsLoading = false;
            }
        }

    }
}