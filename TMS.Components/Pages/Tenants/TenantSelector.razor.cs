using Microsoft.AspNetCore.Components;
using MudBlazor;
using TMS.Shared.Constants;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Pages.Tenants
{
    public partial class TenantSelector
    {
        [Parameter]
        public string SelectedClientId { get; set; } = string.Empty;
        public List<TenantResponse> ClientCollection { get; set; } = new();
        public async void OnSelectionChanged(string clientId)
        {
            var currentClientId =await _localStorage.GetItemAsync<string>(nameof(ApplicationConstants.TenantId));
            if (currentClientId != clientId.ToString())
            {
               await _localStorage.SetItemAsync(nameof(ApplicationConstants.TenantId), clientId);
                _navigationManager.NavigateTo(ApplicationURL.Dashboard, true);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            var result = await _userDataService.GetTenantsByUserIdAsync(CurrentUserId);
            if (result.Succeeded)
            {
                ClientCollection = result.Data;
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