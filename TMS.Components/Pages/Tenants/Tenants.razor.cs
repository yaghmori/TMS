using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TMS.RootComponents.Dialogs;
using TMS.RootComponents.Shared.Components;
using TMS.Shared.Constants;
using TMS.Shared.PagedCollections;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Pages.Tenants
{
    public partial class Tenants
    {
        [CascadingParameter] private HubConnection hubConnection { get; set; }

        private IPagedList<TenantResponse> ClientPagedCollection = new PagedList<TenantResponse>();

        private MudDataGrid<TenantResponse>? _mudDataGrid;
        private string Query { get; set; } = string.Empty;

        protected async override Task OnInitializedAsync()
        {
            _appState.SetAppTitle(_viewLocalizer["Clients"]);
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }
        }

        public async Task AddRemoveUsers(TenantResponse tenant)
        {
            var parameters = new DialogParameters();
            parameters.Add("TenantId", tenant.Id);
            var options = new DialogOptions()
            { CloseButton = true, DisableBackdropClick = true, FullWidth = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = _dialog.Show<TenantUsersDialog>(_viewLocalizer["Client"].Value + " : " + tenant.Name, parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                if (hubConnection is not null)
                {
                    await hubConnection.SendAsync(EndPoints.Hub.SendUpdateAuthState, null);
                }

                await _mudDataGrid.ReloadServerData();
            }
        }

        public async void OpenTenantConfiguration(TenantResponse item)
        {
            var isPersistent = await _localStorage.GetItemAsync<bool>(ApplicationConstants.IsPersistent);
            if (isPersistent)
                await _localStorage.SetItemAsync(ApplicationConstants.TenantId, item.Id);
            else
                await _sessionStorage.SetItemAsync(ApplicationConstants.TenantId, item.Id);

            _navigationManager.NavigateTo(ApplicationURL.Tenants + "/" + item.Id);
        }

        public async Task AddOrUpdateTenant(TenantResponse item)
        {
            var parameters = new DialogParameters();
            var title = "";
            if (item != null)
            {
                parameters.Add("TenantId", item.Id);
                title = _viewLocalizer["UpdateClient"].Value;
                var userResponse = await _TenantDataService.GetUsersByTenantId(item.Id);
                if (userResponse.Succeeded)
                {
                    List<string> users = userResponse.Data.Select(x => x.Id).ToList();
                    if (hubConnection is not null)
                    {
                        await hubConnection.SendAsync(EndPoints.Hub.SendUpdateAuthState, users);
                    }
                }
            }
            else
            {
                parameters.Add("TenantId", string.Empty);
                title = _viewLocalizer["CreateNewClient"].Value;
            }

            var options = new DialogOptions()
            { CloseButton = true, DisableBackdropClick = true, FullWidth = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = _dialog.Show<AddOrUpdateTenantDialog>(title, parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await _mudDataGrid.ReloadServerData();
            }
        }

        public async Task DeleteTenant(TenantResponse tenant)
        {
            var ContentText = _messageLocalizer["DoYouReallyWantToDeleteClient"].Value;
            var ButtonText = _viewLocalizer["Delete"].Value;
            var parameters = new DialogParameters
            {
                { "Title", tenant.Name },
                { "ButtonText", ButtonText },
                { "ContentText", ContentText },
                { "ButtonColor", Color.Error },
                { "ButtonIcon", Icons.Rounded.Delete },
                { "TitleIcon", Icons.Rounded.Delete },
            };
            var options = new DialogOptions()
            {
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraSmall
            };

            //Get list of TenantUsers before delete tenant
            var userResponse = await _TenantDataService.GetUsersByTenantId(tenant.Id);

            var dialog = _dialog.Show<MessageDialog>("", parameters, options);
            var dgResult = await dialog.Result;
            if (!dgResult.Cancelled)
            {

                var deleteDatabaseResponse = await _TenantDataService.DeleteDatabseAsync(tenant.Id);
                if (!deleteDatabaseResponse.Succeeded)
                {
                    foreach (var message in deleteDatabaseResponse.Messages)
                    {
                        _snackbar.Add(message, Severity.Error);
                    }
                }

                var deleteResponse = await _TenantDataService.DeleteTenantByIdAsync(tenant.Id);
                if (deleteResponse != null)
                {
                    //SignalR
                    if (userResponse.Succeeded)
                    {
                        List<string> users = userResponse.Data.Select(x => x.Id).ToList();
                        if (hubConnection is not null)
                        {
                            await hubConnection.SendAsync(EndPoints.Hub.SendUpdateAuthState, users);
                        }
                    }
                    _snackbar.Add(_messageLocalizer["ClientSuccessfullyDeleted"].Value, Severity.Success);
                }
                else
                {
                    _snackbar.Add(_messageLocalizer["DeletingClientFailed"].Value, Severity.Error);
                    foreach (var message in deleteResponse.Messages)
                    {
                        _snackbar.Add(message, Severity.Error);
                    }
                }
                await _mudDataGrid.ReloadServerData();
            }
        }

        private async Task<GridData<TenantResponse>> ReloadDataAsync(GridState<TenantResponse> state)
        {
            IsBusy = true;
            var result = await _TenantDataService.GetTenantsPagedAsync(state.Page, state.PageSize, Query);
            if (result.Succeeded)
            {
                ClientPagedCollection = result.Data;
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
            var clients = new GridData<TenantResponse>
            {
                Items = ClientPagedCollection.Items,
                TotalItems = ClientPagedCollection.TotalCount
            };
            IsBusy = false;

            return clients;
        }

        public async void OnSearchData(string query)
        {
            Query = query;
            await _mudDataGrid.ReloadServerData();
        }

    }
}