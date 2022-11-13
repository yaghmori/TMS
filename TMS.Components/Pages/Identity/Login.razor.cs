using MudBlazor;
using TMS.Core.Application.Authorization;
using TMS.Shared.Constants;
using TMS.Shared.Requests;

namespace TMS.RootComponents.Pages.Identity
{
    public partial class Login
    {
        public SignInByEmailRequest Request = new();
        public bool IsBusy = false;
        public bool PasswordVisibility { get; set; } = false;
        InputType PasswordInput = InputType.Password;
        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        void TogglePasswordVisibility()
        {
            if (PasswordVisibility)
            {
                PasswordVisibility = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                PasswordVisibility = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }

        private async Task OnSubmitAsync()
        {
            var result = await _authDataService.GetTokenAsync(Request);
            if (result.Succeeded)
            {
                await ((AuthStateProvider)_authStateProvider).NotifyLoginAsync(result.Data, Request.IsPersistent);
                _navigationManager.NavigateTo(ApplicationURL.Index, true);

            }
            else
            {
                foreach (var message in result.Messages)
                {
                    _snackbar.Add(message, Severity.Error);
                }
            }
        }

    }
}