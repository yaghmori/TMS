using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Security.Claims;
using TMS.RootComponents.Extensions;
using TMS.Shared.Constants;
using TMS.Shared.Extensions;
using TMS.Shared.Requests;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Shared.Layout
{
    public partial class MainLayout : IAsyncDisposable
    {

        [CascadingParameter] public Task<AuthenticationState>? AuthState { get; set; }
        public bool IsLoading { get; set; } = false;
        public bool IsDrawerOpen { get; set; } = true;
        public ClaimsPrincipal? CurrentUser { get; set; }
        private UserResponse User { get; set; } = new();
        public string ModeIcon => User.Settings.DarkMode ? Icons.Outlined.LightMode : Icons.Outlined.DarkMode;
        public string RTLIcon => User.Settings.RightToLeft ? Icons.Filled.FormatTextdirectionLToR : Icons.Filled.FormatTextdirectionRToL;

        private HubConnection hubConnection;

        public List<BreadcrumbItem> BreadcrumbItems = new();

        protected override async Task OnInitializedAsync()
        {
            string accessToken = string.Empty;
            var isPersistent = await _localStorage.GetItemAsync<bool>(ApplicationConstants.IsPersistent);
            if (isPersistent)
                accessToken = await _localStorage.GetItemAsync<string>(ApplicationConstants.AccessToken);
            else
                accessToken = await _sessionStorage.GetItemAsync<string>(ApplicationConstants.AccessToken);
            if (accessToken == null)
                return;

            hubConnection = hubConnection.TryInitialize(accessToken);

            var authState = await AuthState!;
            var user = authState.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                return;
            }




            await UpdateCureentUserAsync();
            HubSubscription();
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }

        }

        private void HubSubscription()
        {
            hubConnection.On(EndPoints.Hub.ReceiveUpdateAuthState, async () =>
            {
                await _authStateProvider.GetAuthenticationStateAsync();
                await UpdateCureentUserAsync();
                StateHasChanged();
            });
            hubConnection.On(EndPoints.Hub.ReceiveUpdateCulture, async () =>
            {
                _navigationManager.NavigateTo(_navigationManager.Uri, forceLoad: true);
            });
            hubConnection.On(EndPoints.Hub.ReceiveUpdateUser, async () =>
            {
                await UpdateCureentUserAsync();
                StateHasChanged();
            });

            hubConnection.On(EndPoints.Hub.ReceiveTerminateSession, async () =>
            {
                _navigationManager.NavigateTo(ApplicationURL.Logout);
            });

        }

        private async Task UpdateCureentUserAsync()
        {
            var state = await AuthState!;
            CurrentUser = state.User;

            var userId = state.User.GetUserId();
            var result = await _userDataService.GetUserByIdAsync(userId);
            if (result.Succeeded)
            {
                User = result.Data;
                StateHasChanged();
            }
        }

        private void DrawerToggle()
        {
            IsDrawerOpen = !IsDrawerOpen;
        }

        private async void DarkModeToggle()
        {
            User.Settings.DarkMode = !User.Settings.DarkMode;
            var patchDoc = new JsonPatchDocument<UserSettingsRequest>();
            patchDoc.Replace(e => e.DarkMode, User.Settings.DarkMode);
            var result = await _userSettingDataService.UpdateSettingsAsync(User.Settings.Id, patchDoc);

            //SignalR
            var users = new List<string> { User.Id };
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync(EndPoints.Hub.SendUpdateUser, users);
            }

        }

        private async void RtlToggle()
        {
            User.Settings.RightToLeft = !User.Settings.RightToLeft;
            var patchDoc = new JsonPatchDocument<UserSettingsRequest>();
            patchDoc.Replace(e => e.RightToLeft, User.Settings.RightToLeft);
            var result = await _userSettingDataService.UpdateSettingsAsync(User.Settings.Id, patchDoc);

            //SignalR
            var users = new List<string> { User.Id };
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync(EndPoints.Hub.SendUpdateUser, users);
            }
            //  await OnUserUpdated.InvokeAsync(User);
        }

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            var relativePath = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
            BreadcrumbItems.Clear();
            var list = relativePath.Split('/');
            foreach (var item in list)
            {
                BreadcrumbItems.Add(new BreadcrumbItem(item, href: item));
            }

            StateHasChanged();
        }

        public async ValueTask DisposeAsync()
        {
            if (hubConnection is not null)
            {
                await hubConnection.DisposeAsync();
            }
        }

    }
}