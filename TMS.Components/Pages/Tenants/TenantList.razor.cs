using Microsoft.AspNetCore.Components;
using MudBlazor;
using TMS.Shared.Constants;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Pages.Tenants
{
    public partial class TenantList
    {
        [Parameter]
        public string SelectedClientId { get; set; } = string.Empty;
        public List<TenantResponse> ClientCollection { get; set; } = new();
        async void OnSelectionChanged(TenantResponse client)
        {
            var isPersistent = await _localStorage.GetItemAsync<bool>(ApplicationConstants.IsPersistent);
            if (isPersistent)
                await _localStorage.SetItemAsync(ApplicationConstants.TenantId, client.Id);
            else
                await _sessionStorage.SetItemAsync(ApplicationConstants.TenantId, client.Id);
            _navigationManager.NavigateTo(ApplicationURL.Configuration + "/" + client.Name);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();
            var result = await _TenantDataService.GetTenantsAsync();
            if(result.Succeeded)
            {
                ClientCollection = result.Data.ToList();
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
        }
    }
}