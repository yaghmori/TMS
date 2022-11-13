using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TMS.RootComponents.Interfaces;
using TMS.Shared.Constants;

namespace TMS.RootComponents.Shared
{
    public partial class BaseComponent : ComponentBase,IAuthBase
    {
        [CascadingParameter]
        public Task<AuthenticationState>? AuthState { get; set; }
        public bool IsBusy { get; set; } = false;
        public bool IsLoading { get; set; } = false;
        public bool IsSaving { get; set; } = false;
        public bool HasInit { get; set; } = false;
        public bool IsAuthenticated => CurrentUser?.Identity?.IsAuthenticated ?? false;
        public string CurrentUserId => CurrentUser?.FindFirst(x => x.Type == ApplicationClaimTypes.UserId)?.Value ?? string.Empty;
        public string CurrentUserEmail => CurrentUser?.FindFirst(x => x.Type == ApplicationClaimTypes.Email)?.Value ?? string.Empty;
        public ClaimsPrincipal? CurrentUser { get; set; }


        protected async override Task OnParametersSetAsync()
        {
            //CurrentUser= await ((AuthStateProvider)_authStateProvider).CurrentUser();
            //await ((AuthStateProvider)_authStateProvider).StateChangedAsync();
            var state = await AuthState!;
            CurrentUser = state.User;
            //StateHasChanged();

        }

    }
}
