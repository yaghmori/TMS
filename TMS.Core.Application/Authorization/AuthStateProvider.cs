using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Security.Claims;
using TMS.Core.Application.DataServices;
using TMS.Core.Shared.Helpers;
using TMS.Shared.Constants;
using TMS.Shared.Helpers;
using TMS.Shared.Responses;
using static TMS.Shared.Constants.ApplicationPermissions;

namespace TMS.Core.Application.Authorization
{


    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;

        private readonly IUserDataService _userDataService;
        private readonly IRoleDataService _roleDataService;
        private readonly AuthenticationState _anonymous;

        public AuthStateProvider(ILocalStorageService localStorageService, IUserDataService userDataService, ISessionStorageService sessionStorage, IRoleDataService roleDataService)
        {
            _localStorage = localStorageService;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            _userDataService = userDataService;
            _sessionStorage = sessionStorage;
            _roleDataService = roleDataService;
        }
        public async Task StateChangedAsync()
        {
            var authState = Task.FromResult(await GetAuthenticationStateAsync());
            NotifyAuthenticationStateChanged(authState);

        }
        public async Task<ClaimsPrincipal> CurrentUser()
        {
            var authState = Task.FromResult(await GetAuthenticationStateAsync());
            NotifyAuthenticationStateChanged(authState);
            return authState.Result.User;
        }


        public async Task NotifyLoginAsync(TokenResponse token, bool isPersistent)
        {
            await ClearLocalStorage();

            await _localStorage.SetItemAsync(ApplicationConstants.IsPersistent, isPersistent);

            if (isPersistent)
            {
                await _localStorage.SetItemAsync(ApplicationConstants.AccessToken, token.AccessToken);
                await _localStorage.SetItemAsync(ApplicationConstants.RefreshToken, token.RefreshToken);

            }
            else
            {
                await _sessionStorage.SetItemAsync(ApplicationConstants.AccessToken, token.AccessToken);
                await _sessionStorage.SetItemAsync(ApplicationConstants.RefreshToken, token.RefreshToken);
            }
            var authenticatedUser = new ClaimsPrincipal(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.GetClaimsFromJwt(token.AccessToken), "jwtAuthType")));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }
        public async Task NotifyLogoutAsync()
        {
            await ClearLocalStorage();
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }

        private async Task ClearLocalStorage()
        {
            await _localStorage.ClearAsync();
            await _sessionStorage.ClearAsync();

        //    await _localStorage.RemoveItemAsync(ApplicationConstants.AccessToken);
        //    await _localStorage.RemoveItemAsync(ApplicationConstants.RefreshToken);
        //    await _localStorage.RemoveItemAsync(ApplicationConstants.TenantId);
        //    await _localStorage.RemoveItemAsync(ApplicationConstants.Culture);
        //    await _localStorage.RemoveItemAsync(ApplicationConstants.IsPersistent);
        //    //=========SessionStorage
        //    await _sessionStorage.RemoveItemAsync(ApplicationConstants.AccessToken);
        //    await _sessionStorage.RemoveItemAsync(ApplicationConstants.RefreshToken);
        //    await _sessionStorage.RemoveItemAsync(ApplicationConstants.TenantId);
        //    await _sessionStorage.RemoveItemAsync(ApplicationConstants.Culture);
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = string.Empty;

            var isPersistent = await _localStorage.GetItemAsync<bool>(ApplicationConstants.IsPersistent);
            if (isPersistent)
                savedToken = await _localStorage.GetItemAsync<string>(ApplicationConstants.AccessToken);
            else
                savedToken = await _sessionStorage.GetItemAsync<string>(ApplicationConstants.AccessToken);

            if (string.IsNullOrWhiteSpace(savedToken))
            {
                await ClearLocalStorage();
                return _anonymous;
            }

            //TODO : Validate JWT
            //TODO : Make all requests in one request

            //GetJWTClaims
            var jwtClaims = JwtParser.GetClaimsFromJwt(savedToken);
            var userId = jwtClaims.FirstOrDefault(x => x.Type.Equals(ApplicationClaimTypes.UserId))?.Value!;
            if (string.IsNullOrWhiteSpace(userId))
            {
                await ClearLocalStorage();
                return _anonymous;
            }


            //GetUserClaims
            var userClaims = new List<Claim>();
            var permissionResponse = await _userDataService.GetClaimsByUserIdAsync(userId);
            if (permissionResponse.Succeeded)
                userClaims = permissionResponse.Data.Select(s => new Claim(s.ClaimType, s.ClaimValue)).ToList();

            //GetRoleClaims
            var roleClaims = new List<Claim>();
            var roleResponse = await _userDataService.GetUserRolesByUserIdAsync(userId);
            if (roleResponse.Succeeded)
            {
                foreach (var role in roleResponse.Data)
                {
                    roleClaims.Add(new Claim(ApplicationClaimTypes.Role, role.Name));
                    var roleClaimsResponse = await _roleDataService.GetClaimsByRoleIdAsync(role.Id);
                    if (roleClaimsResponse.Succeeded)
                    {
                        roleClaims.AddRange(roleClaimsResponse.Data.Select(s => new Claim(s.ClaimType, s.ClaimValue)).ToList());
                    }

                }
            }

            //GetTenants
            var tenantClaims = new List<Claim>();
            var clientResponse = await _userDataService.GetTenantsByUserIdAsync(userId);
            if (clientResponse.Succeeded)
                tenantClaims = clientResponse.Data.Select(s => new Claim(ApplicationClaimTypes.Tenant, s.Id)).ToList();



            var claims = new List<Claim>()
                .Union(jwtClaims, new ClaimComparer())
                .Union(roleClaims, new ClaimComparer())
                .Union(userClaims, new ClaimComparer())
                .Union(tenantClaims, new ClaimComparer());



            var claimIdentity = new ClaimsIdentity(claims, "jwt");
            var state = new AuthenticationState(new ClaimsPrincipal(claimIdentity));
            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;




        }

    }

}
