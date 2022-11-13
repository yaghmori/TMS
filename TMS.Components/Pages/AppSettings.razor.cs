using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using TMS.Shared.Helpers;
using TMS.Shared.Models;

namespace TMS.RootComponents.Pages
{
    public partial class AppSettings
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        [Parameter]
        public string RoleId { get; set; } = string.Empty;
        public JwtSettings JwtSetting { get; set; } = new();
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        protected async override Task OnInitializedAsync()
        {
            IsLoading = true;
            IsLoading = false;
            StateHasChanged();
        }

        public async void SaveChangesSubmit(EditContext context)
        {
            IsBusy = true;
            ConfigHelper.SetJwtSettings(JwtSetting);
            _snackbar.Add(_messageLocalizer["RoleSuccessfullyUpdated"].Value, Severity.Success);
            IsBusy = false;
        }
        protected override async Task OnParametersSetAsync()
        {
            //(await _authStateProvider.GetAuthenticationStateAsync()).;
            //_canViewDashboard = (await _authorizationService.AuthorizeAsync(CurrentUser, ApplicationPermissions.Dashboards.View)).Succeeded;
            _appState.AppTitle = _viewLocalizer["AppSettings"];
            await base.OnParametersSetAsync();
        }

    }
}

