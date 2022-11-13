using Microsoft.AspNetCore.Components;
using MudBlazor;
using TMS.Shared.Constants;
using TMS.Shared.Requests;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Dialogs
{
    public partial class RolePermissionsDialog
    {
        [Parameter]
        public string RoleId { get; set; } = string.Empty;
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        private MudDataGrid<UserClaimRequest>? _mudDataGrid = new();

        private HashSet<string> SelectedPermission { get; set; } = new HashSet<string>();
        public IEnumerable<string> PermissionCollection { get; set; } = new List<string>();
        private string Query { get; set; } = string.Empty;


        private bool InProcess = false;
        //private bool CanAddUser { get => User != null && !Users.Any(x => x.Id == User.Id); }
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        async void SaveChanges()
        {
            InProcess = true;
            var result = await _roleDataService.UpdateRoleClaimsAsync(RoleId, SelectedPermission.Select(s => new ClaimResponse
            {
                ClaimType = ApplicationClaimTypes.Permission,
                ClaimValue = s
            }).ToList());

            if (!result.Succeeded)
            {
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Success);
                }
            }
            InProcess = false;
            MudDialog.Close(DialogResult.Ok(result.Succeeded));
        }


        private bool FilterFunc(string item)
        {
            if (string.IsNullOrWhiteSpace(Query))
                return true;
            if (item.Contains(Query, StringComparison.InvariantCultureIgnoreCase))
                return true;
            return false;
        }
        void OnSearchData(string query)
        {
            Query = query;
            _mudDataGrid.ReloadServerData();
        }

        protected async override Task OnInitializedAsync()
        {
            PermissionCollection = new List<string>();
            SelectedPermission = new HashSet<string>();

            PermissionCollection = new HashSet<string>(ApplicationPermissions.GetRegisteredPermissions());

            var claimResult = await _roleDataService.GetClaimsByRoleIdAsync(RoleId);
            if (claimResult.Succeeded)
            {
                SelectedPermission = new HashSet<string>(
                    claimResult.Data.Where(x => x.ClaimType.Equals( ApplicationClaimTypes.Permission)).Select(x => x.ClaimValue));
            }

        }
    }
}