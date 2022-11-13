using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using TMS.Shared.Requests;

namespace TMS.RootComponents.Dialogs
{
    public partial class CreateNewUserDialog
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }
        public NewUserRequest NewUser { get; set; } = new();
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        async void SaveChangesSubmit(EditContext context)
        {
            IsBusy = true;

            var result = await _userDataService.AddNewUserWithPasswordAsync(NewUser);
            if (result.Succeeded)
            {
                _snackbar.Add(_messageLocalizer["UserSuccessfullyCreated"].Value, Severity.Success);
            }
            else
            {
                _snackbar.Add(_messageLocalizer["CreatingUserFailed"].Value, Severity.Error);
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }


            IsBusy = false;
            MudDialog.Close(DialogResult.Ok(result.Data));
        }
    }
}