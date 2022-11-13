using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using TMS.Shared.Constants;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Pages
{
    public partial class Chat
    {
        private List<string> messages = new List<string>();
        private string? userInput;
        private string? messageInput;

        [Parameter]
        public string RoleId { get; set; } = string.Empty;
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection hubConnection { get; set; }
        [CascadingParameter] private UserResponse User { get; set; }

        private IEnumerable<string> SelectedUsers { get; set; } = new List<string>();
        public List<UserResponse> UserCollection { get; set; } = new();
        private bool InProcess = false;
        //private bool CanAddUser { get => User != null && !Users.Any(x => x.Id == User.Id); }


        private string GetMultiSelectionText(List<string> selectedValues)
        {
            SelectedUsers = SelectedUsers.ToList();
            return _viewLocalizer["UserSelected", selectedValues.Count].Value;
        }

        public async void RemoveUser(MudChip chip)
        {
            SelectedUsers = SelectedUsers.Where(x => x != chip.Value.ToString()).ToList();
        }

        protected override async Task OnInitializedAsync()
        {
            var userCollectionResult = await _userDataService.GetUsersAsync();
            if (userCollectionResult.Succeeded)
            {
                UserCollection = userCollectionResult.Data;
            }
            var selectedUsersResult = await _roleDataService.GetUsersByRoleIdAsync(RoleId);
            if (selectedUsersResult.Succeeded)
            {
                SelectedUsers = selectedUsersResult.Data.Select(x => x.Id).ToList();
            }

            hubConnection.On<string, string>(EndPoints.Hub.ReceiveMessage, async (sender, message) =>
            {
                var encodedMsg = $"{sender} : {message}";
                messages.Add(encodedMsg);
                StateHasChanged();
            });

            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }

        }

        private async Task Send()
        {
            if (hubConnection is not null)
            {
                await hubConnection.SendAsync(EndPoints.Hub.SendMessage,User.Email,messageInput, SelectedUsers);
            }
        }

        public bool CanSendMessage => hubConnection?.State == HubConnectionState.Connected && !string.IsNullOrWhiteSpace(messageInput);
    }
}