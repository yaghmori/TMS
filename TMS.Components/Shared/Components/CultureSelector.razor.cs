using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;
using System.Security.Claims;
using TMS.Shared.Constants;
using TMS.Shared.Extensions;
using TMS.Shared.Requests;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Shared.Components
{
    public partial class CultureSelector
    {
        [CascadingParameter] private UserResponse User { get; set; }
        [CascadingParameter] public HubConnection hubConnection { get; set; }

        [Parameter] public string Class { get; set; } = string.Empty;

        public List<CultureResponse> Cultures { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var result = await _cultureDataService.GetCulturesAsync();
            if (result.Succeeded)
            {
                Cultures = result.Data;
            }
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }

        }
        private async void OnClick(CultureResponse culture)
        {
            var patchDoc = new JsonPatchDocument<UserSettingsRequest>();
            patchDoc.Replace(e => e.Culture, culture.CultureInfo.Name);
            patchDoc.Replace(e => e.RightToLeft, culture.RightToLeft);
            var result = await _userSettingDataService.UpdateSettingsAsync(User.Settings.Id, patchDoc);
            if (result.Succeeded)
            {
                //TODO : Implement LocalStorage Service
                var isPersistent = await _localStorage.GetItemAsync<bool>(ApplicationConstants.IsPersistent);
                if (isPersistent)
                    await _localStorage.SetItemAsync(ApplicationConstants.Culture, culture.CultureInfo.Name);
                else
                    await _sessionStorage.SetItemAsync(ApplicationConstants.Culture, culture.CultureInfo.Name);

                //SignalR
                var users = new List<string> { User.Id };
                if (hubConnection is not null)
                {
                    await hubConnection.SendAsync(EndPoints.Hub.SendUpdateAuthState, users);
                }
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