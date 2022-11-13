using Microsoft.AspNetCore.Components;
using MudBlazor;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Dialogs
{
    public partial class UserRolesDialog
    {
        [Parameter]
        public string UserId { get; set; } = string.Empty;
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        private IEnumerable<string> SelectedRoles { get; set; } = new List<string>();
        public List<RoleResponse> RoleCollection { get; set; } = new();
        private bool InProcess = false;
        //private bool CanAddUser { get => User != null && !Users.Any(x => x.Id == User.Id); }
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        async void SaveChanges()
        {
            InProcess = true;
            var result = await _roleDataService.UpdateUserRolesAsync(UserId.ToString(), SelectedRoles.ToList());
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
            SelectedRoles = SelectedRoles.ToList();
            return _viewLocalizer["RoleSelected", selectedValues.Count].Value;
        }

        public async void RemoveRole(MudChip chip)
        {
            SelectedRoles = SelectedRoles.Where(x => x !=chip.Value.ToString()).ToList();
        }

        protected async override Task OnInitializedAsync()
        {
            var roleCollectionResult = await _roleDataService.GetRolesAsync();
            if (roleCollectionResult.Succeeded)
            {
                RoleCollection = roleCollectionResult.Data;
            }
            var selectedRolesResult = await _userDataService.GetUserRolesByUserIdAsync(UserId);
            if (selectedRolesResult.Succeeded)
            {
                SelectedRoles = selectedRolesResult.Data.Select(x => x.Id).ToList();
            }

        }
    }
}