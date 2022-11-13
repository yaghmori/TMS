using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TMS.Shared.Constants;
using TMS.Shared.Requests;

namespace TMS.RootComponents.Dialogs
{
    public partial class AddOrUpdateRoleDialog
    {
        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] public HubConnection hubConnection { get; set; }
        [Parameter] public string RoleId { get; set; } = string.Empty;
        public RoleRequest Role { get; set; } = new();

        protected async override Task OnInitializedAsync()
        {
            IsLoading = true;
            if (!string.IsNullOrWhiteSpace(RoleId))
            {
                var result = await _roleDataService.GetRoleByIdAsync(RoleId);
                if (result.Succeeded)
                {
                    Role = _autoMapper.Map<RoleRequest>(result.Data);
                }
                else
                {
                    foreach (var message in result.Messages)
                    {
                        _snackbar.Add(message, Severity.Error);
                    }
                }
            }
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }
            IsLoading = false;
            StateHasChanged();
        }
        private async void SaveChangesSubmit(EditContext context)
        {
            IsBusy = true;
            bool dgResult = false;
            if (string.IsNullOrWhiteSpace(RoleId)) //New
            {
                var result = await _roleDataService.AddRoleAsync(Role.Name);
                if (result.Succeeded)
                {
                    var roleId = result.Data;
                    if (!string.IsNullOrWhiteSpace(roleId))
                    {
                        dgResult = true;
                        _snackbar.Add(_messageLocalizer["RoleSuccessfullyCreated"].Value, Severity.Success);
                    }
                }
                else
                {
                    _snackbar.Add(_messageLocalizer["CreatingRoleFailed"].Value, Severity.Error);
                    foreach (var message in result.Messages)
                    {
                        _snackbar.Add(message, Severity.Success);
                    }
                }
            }
            else //Edit
            {
                var result = await _roleDataService.UpdateRoleByIdAsync(RoleId, Role.Name);
                if (result.Succeeded)
                {
                    dgResult = true;
                    _snackbar.Add(_messageLocalizer["RoleSuccessfullyUpdated"].Value, Severity.Success);
                    var usersResponse = await _roleDataService.GetUsersByRoleIdAsync(RoleId);
                    if (usersResponse.Succeeded)
                    {
                        //SignalR
                        var users = usersResponse.Data.Select(x => x.Id).ToList();
                        if (hubConnection is not null)
                        {
                            await hubConnection.SendAsync(EndPoints.Hub.SendUpdateAuthState, users);
                        }
                    }


                }
                else
                {
                    _snackbar.Add(_messageLocalizer["UpdatingRoleFailed"].Value, Severity.Error);
                    foreach (var message in result.Messages)
                    {
                        _snackbar.Add(message, Severity.Success);
                    }

                }
            }

            IsBusy = false;
            MudDialog.Close(DialogResult.Ok(dgResult));
        }
        private void Cancel()
        {
            MudDialog.Cancel();
        }

    }
}