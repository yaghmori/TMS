using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TMS.Shared.Constants;
using TMS.Shared.Requests;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Pages.Identity
{
    public partial class UserProfile
    {

        [CascadingParameter] private UserResponse User { get; set; } = new();
        private UserResponse UserRequest { get; set; } = new();

        private ChangePasswordRequest changePasswordRequest { get; set; } = new();
        [CascadingParameter] private HubConnection hubConnection { get; set; }

        protected async override Task OnInitializedAsync()
        {
            IsLoading = true;
            UserRequest = (await _userDataService.GetUserByIdAsync(User.Id)).Data;
            _appState.SetAppTitle(_viewLocalizer["Profile"]);
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }

            IsLoading = false;
            StateHasChanged();
        }
        private async void SaveChangesSubmit(EditContext context)
        {
            IsSaving = true;
            var patchDoc = new JsonPatchDocument<UserRequest>();
            patchDoc.Replace(e => e.FirstName, UserRequest.FirstName);
            patchDoc.Replace(e => e.LastName, UserRequest.LastName);
            patchDoc.Replace(e => e.Description, UserRequest.Description);
            //var request = _autoMapper.Map<UserRequest>(User);
            var result = await _userDataService.UpdateUserByIdAsync(UserRequest.Id, patchDoc);
            if (result.Succeeded)
            {
                context.MarkAsUnmodified();

                //SignalR
                List<string> users = new List<string> { UserRequest.Id };
                if (hubConnection is not null)
                {
                    await hubConnection.SendAsync(EndPoints.Hub.SendUpdateAuthState, users);
                }

                _snackbar.Add(_messageLocalizer["UserAccountUpdatedSuccessfully"], Severity.Success);
            }
            else
            {
                _snackbar.Add(_messageLocalizer["UserAccountNotUpdated"], Severity.Error);
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
            IsSaving = false;
            StateHasChanged();
        }
        private async void ChangePasswordSubmit(EditContext context)
        {
            IsSaving = true;
            var result = await _userDataService.ChangePasswordAsync(UserRequest.Id, changePasswordRequest);
            if (result.Succeeded)
            {
                context.MarkAsUnmodified();
                _snackbar.Add(_messageLocalizer["UserPasswordChangeSuccessfully"], Severity.Success);
                changePasswordRequest = new();
            }
            else
            {
                _snackbar.Add(_messageLocalizer["UserPassworNotUpdated"], Severity.Error);
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
            IsSaving = false;
            StateHasChanged();
        }
    }
}