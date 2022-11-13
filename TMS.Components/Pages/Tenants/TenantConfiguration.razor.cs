using Microsoft.AspNetCore.Components;
using MudBlazor;
using TMS.Core.Domain.Enums;
using TMS.RootComponents.Dialogs;
using TMS.RootComponents.Shared.Components;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Pages.Tenants
{
    public partial class TenantConfiguration
    {
        [Parameter]
        public string TenantId { get; set; } = string.Empty;
        public TenantResponse Tenant { get; set; } = new();


        public SiloItemResponse SelectedSiloItem { get; set; } = new();

        private TreeView _treeViewComponent;

        private int Index1 = -1; //default value cannot be 0 -> first selectedindex is 0.

        public List<ChartSeries> Series1 = new List<ChartSeries>()
        {
        new ChartSeries() { Name = "Series 1", Data = new double[] { 90, 79, 72, 69, 62, 62, 55, 65, 70 } },
        new ChartSeries() { Name = "Series 2", Data = new double[] { 10, 41, 35, 51, 49, 62, 69, 91, 148 } },
        };
        public string[] XAxisLabels1 = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

        private void SelectedSiloItemChanged(SiloItemResponse siloItem)
        {
            SelectedSiloItem = siloItem;
        }

        private async Task AddSilo()
        {
            var parameters = new DialogParameters
            {
                { "ParentId",  null  },
                { "ItemType", SiloItemTypeEnum.Silo }
            };

            var title = _viewLocalizer["Add"].Value + " " + SiloItemTypeEnum.Silo.ToString();

            var options = new DialogOptions()
            { CloseButton = true, DisableBackdropClick = true, FullWidth = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = _dialog.Show<AddSiloItemDialog>(title, parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await _treeViewComponent.ReloadData();
            }
        }

        protected async override Task OnParametersSetAsync()
        {
            IsLoading = true;
            var tenantResponse = await _TenantDataService.GetTenantByIdAsync(TenantId);
            if (tenantResponse.Succeeded)
            {
                Tenant = tenantResponse.Data;
                _appState.SetAppTitle(Tenant.Name);
            }
            else
            {
                foreach (var message in tenantResponse.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
            IsLoading = false;
        }


    }
}
