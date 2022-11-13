using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TMS.Core.Application.DataServices;
using TMS.Shared.Constants;
using TMS.Shared.Helpers;

namespace TMS.Core.Application.Authorization
{
    public class ApplicationClaimsTransformation : IClaimsTransformation
    {
        private readonly IUserDataService _userDataService;
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;

        public ApplicationClaimsTransformation(IUserDataService userDataService, ILocalStorageService localStorage, ISessionStorageService sessionStorage)
        {
            _userDataService = userDataService;
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
        }


        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            //var isPersistent = await _localStorage.GetItemAsync<bool>(ApplicationSettings.IsPersistent);
            //var savedToken = string.Empty;

            //if (isPersistent)
            //    savedToken = await _localStorage.GetItemAsync<string>(ApplicationSettings.AccessToken);
            //else
            //    savedToken = await _sessionStorage.GetItemAsync<string>(ApplicationSettings.AccessToken);

            //if (string.IsNullOrWhiteSpace(savedToken))
            //{
            //    return principal;
            //}

            //var result = await _userDataService.GetClaimsByJwtAsync(savedToken);
            //if (result.Succeeded)
            //{
            //    var permissions = result.Data.Select(s => new Claim(ApplicationClaimTypes.Permission, s.ClaimValue)).AsEnumerable();
            //    var claims = new ClaimsIdentity(JwtParser.GetClaimsFromJwt(savedToken), "jwtAuthType");
            //    claims.AddClaims(permissions);
            //    return new ClaimsPrincipal(claims);
            //}
            //else
            //{
                return principal;
            //}

        }
    }
}
