using Microsoft.AspNetCore.Components;
using MudBlazor;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Dialogs
{
    public partial class TenantUsersDialog
    {
        [Parameter]
        public string TenantId { get; set; } = string.Empty;
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        private IEnumerable<string> SelectedUsers { get; set; } = new List<string>();
        public List<UserResponse> UserCollection { get; set; } = new();
        private bool InProcess = false;
        //private bool CanAddUser { get => User != null && !Users.Any(x => x.Id == User.Id); }
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        async void SaveChanges()
        {
            InProcess = true;
            var result = await _TenantDataService.ReplaceTenantUsersAsync(TenantId, SelectedUsers.ToList());
            if (result.Succeeded)
            {
            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Success);
                }
            }
            InProcess = false;
            MudDialog.Close(DialogResult.Ok(result.Succeeded));

        }

        private string GetMultiSelectionText(List<string> selectedValues)
        {
            SelectedUsers = SelectedUsers.ToList();
            return _viewLocalizer["UserSelected", selectedValues.Count].Value;
        }

        public async void RemoveUser(MudChip chip)
        {
            SelectedUsers = SelectedUsers.Where(x => x != chip.Value.ToString()).ToList();
        }

        protected async override Task OnInitializedAsync()
        {
            var users =await _userDataService.GetUsersAsync();
            if (users.Succeeded)
            {
                UserCollection = users.Data;
            }
            var selectedUsers = await _TenantDataService.GetUsersByTenantId(TenantId);
            if (selectedUsers.Succeeded)
            {
                SelectedUsers = selectedUsers.Data.Select(x => x.Id).ToList();
            }
        }
    }
}