using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TMS.RootComponents.Shared.Components;
using TMS.Shared.Constants;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Pages.Identity
{
    public partial class UserSessions
    {
        [Parameter] public EventCallback<bool> OnDrawerToggle { get; set; }
        [CascadingParameter] public HubConnection hubConnection { get; set; }
        [Parameter] public string UserId { get; set; } = string.Empty;

        private List<UserSessionResponse> ActiveSessionCollection = new List<UserSessionResponse>();


        protected async override Task OnInitializedAsync()
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }
            await LoadData();
        }



        private async Task LoadData()
        {
            IsBusy = true;
            var result =  await _userDataService.GetUserSessionsAsync(UserId);
            if (result.Succeeded)
            {
                ActiveSessionCollection = result.Data;
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
            IsBusy = false;
        }

        private async Task TerminateSession(UserSessionResponse session)
        {
            var parameters = new DialogParameters
            {
                { "Title", session.Name },
                { "ButtonText", _viewLocalizer["Terminate"].Value },
                { "ContentText", _messageLocalizer["AreYouSureYouWantToTerminateThisSession"].Value },
                { "ButtonColor", Color.Error },
                { "ButtonIcon", Icons.Rounded.Delete },
                { "TitleIcon", Icons.Rounded.Delete },
            };
            var options = new DialogOptions()
            {
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraSmall
            };
            var dialog = _dialog.Show<MessageDialog>("", parameters, options);
            var dgResult = await dialog.Result;
            if (!dgResult.Cancelled)
            {
                
                var result = await _userDataService.TerminateSessionAsync(session.Id);
                if (result.Succeeded)
                {
                    //SignalR
                    var users = new List<string> { session.UserId };
                    if (hubConnection is not null)
                    {
                        await hubConnection.SendAsync(EndPoints.Hub.SendTerminateSession, users);
                    }
                    _snackbar.Add(_messageLocalizer["SessionSuccessfullyTerminated"].Value, Severity.Success);
                    await LoadData();
                }
                else
                {
                    _snackbar.Add(_messageLocalizer["TerminatingSessionFailed"].Value, Severity.Error);
                    foreach (var message in result.Messages)
                    {
                        _snackbar.Add(message, Severity.Error);
                    }
                }
            }
        }



        protected override async Task OnParametersSetAsync()
        {
            _appState.AppTitle = _viewLocalizer["UserSessions"];
            await base.OnParametersSetAsync();
        }

    }
}