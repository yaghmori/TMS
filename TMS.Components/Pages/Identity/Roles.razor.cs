using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TMS.RootComponents.Dialogs;
using TMS.RootComponents.Shared.Components;
using TMS.Shared.Constants;
using TMS.Shared.PagedCollections;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Pages.Identity
{
    public partial class Roles
    {
        [CascadingParameter] private HubConnection hubConnection { get; set; }
        private IPagedList<RoleResponse> RolesPagedCollection = new PagedList<RoleResponse>();
        private MudDataGrid<RoleResponse>? _mudDataGrid = new();
        private string Query { get; set; } = string.Empty;



        protected async override Task OnInitializedAsync()
        {
            _appState.SetAppTitle(_viewLocalizer["Roles"]);
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }
        }
        private async Task AddOrRemoveUsers(RoleResponse role)
        {
            var parameters = new DialogParameters();
            parameters.Add("RoleId", role.Id);
            var options = new DialogOptions()
            { CloseButton = true, DisableBackdropClick = true, FullWidth = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = _dialog.Show<RoleUsersDialog>(role.Name, parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                //SignalR
                if (hubConnection is not null)
                {
                    await hubConnection.SendAsync(EndPoints.Hub.SendUpdateAuthState, null);
                }

                await _mudDataGrid.ReloadServerData();
            }
        }
        private async Task AddOrRemoveRoleClaims(RoleResponse role)
        {
            var parameters = new DialogParameters();
            parameters.Add("RoleId", role.Id);
            var options = new DialogOptions()
            { CloseButton = true, DisableBackdropClick = true, FullWidth = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = _dialog.Show<RolePermissionsDialog>(role.Name, parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                //SignalR
                var usersResponse = await _roleDataService.GetUsersByRoleIdAsync(role.Id);
                if (usersResponse.Succeeded)
                {
                    List<string> users = usersResponse.Data.Select(x => x.Id).ToList();
                    if (hubConnection is not null)
                    {
                        await hubConnection.SendAsync(EndPoints.Hub.SendUpdateAuthState, users);
                    }
                }

                await _mudDataGrid.ReloadServerData();
            }
        }
        private async Task AddOrUpdateRole(RoleResponse item)
        {
            var parameters = new DialogParameters();
            var title = "";
            IReadOnlyList<string> users = new List<string>();

            if (item == null) //New
            {
                parameters.Add("RoleId", string.Empty);
                title = _viewLocalizer["AddNewRole"].Value;
            }
            else //Edit
            {
                parameters.Add("RoleId", item.Id);
                var usersResponse = await _roleDataService.GetUsersByRoleIdAsync(item.Id);
                if (usersResponse.Succeeded)
                {
                    users = usersResponse.Data.Select(x => x.Id).ToList();
                }
                title = _viewLocalizer["UpdateRole"].Value;
            }

            var options = new DialogOptions()
            { CloseButton = true, DisableBackdropClick = true, FullWidth = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = _dialog.Show<AddOrUpdateRoleDialog>(title, parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await _mudDataGrid.ReloadServerData();
            }
        }
        private async Task DeleteRole(RoleResponse role)
        {
            var parameters = new DialogParameters
            {
                { "Title", role.Name },
                { "ButtonText", _viewLocalizer["DeleteRole"].Value },
                { "ContentText", _messageLocalizer["DoYouReallyWantToDeleteRole", role.Name].Value },
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
                //Get list of RoleUsers before delete role
                var usersResponse = await _roleDataService.GetUsersByRoleIdAsync(role.Id);

                var result = await _roleDataService.DeleteRoleByIdAsync(role.Id);
                if (result.Succeeded)
                {
                    //SignalR
                    if (usersResponse.Succeeded)
                    {
                        List<string> users = usersResponse.Data.Select(x => x.Id).ToList();
                        if (hubConnection is not null)
                        {
                            await hubConnection.SendAsync(EndPoints.Hub.SendUpdateAuthState, users);
                        }
                    }

                    _snackbar.Add(_messageLocalizer["RoleSuccessfullyDeleted"].Value, Severity.Success);
                    await _mudDataGrid.ReloadServerData();
                }
                else
                {
                    foreach (var message in result.Messages)
                    {
                        _snackbar.Add(message, Severity.Error);
                        //_snackbar.Add(_messageLocalizer["DeletingRoleFailed"].Value, Severity.Error);

                    }
                }
            }
        }
        private async Task<GridData<RoleResponse>> ReloadDataAsync(GridState<RoleResponse> state)
        {
            IsBusy = true;
            var result = await _roleDataService.GetRolesPagedAsync(state.Page, state.PageSize, Query);
            if (result.Succeeded)
            {
                RolesPagedCollection = result.Data;
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
            var roles = new GridData<RoleResponse>
            {
                Items = RolesPagedCollection.Items,
                TotalItems = RolesPagedCollection.TotalCount
            };
            IsBusy = false;

            return roles;


        }
        private void OnSearchData(string query)
        {
            Query = query;
            _mudDataGrid.ReloadServerData();
        }
    }
}