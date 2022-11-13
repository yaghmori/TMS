using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using TMS.Shared.Constants;
using TMS.Shared.Extensions;
using TMS.Shared.PagedCollections;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;

namespace TMS.Core.Application.DataServices
{


    public class UserDataService : DataServiceBase, IUserDataService
    {

        public UserDataService(IHttpClientFactory httpClient) : base(httpClient)
        {
        }

        public async Task<IResult<List<UserResponse>>> GetUsersAsync(string? query = null)
        {
            var uri = EndPoints.UserController.GetUsers;
            uri = QueryHelpers.AddQueryString(uri, "paged", "false");
            if (!string.IsNullOrWhiteSpace(query))
                uri = QueryHelpers.AddQueryString(uri, nameof(query), query);

            List<UserResponse> result = new List<UserResponse>();
            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<List<UserResponse>>();

        }

        public async Task<IResult<IPagedList<UserResponse>>> GetUsersPagedAsync(int page = 0, int pageSize = 10, string query = null)
        {
            var uri = EndPoints.UserController.GetUsers;

            //uri = QueryHelpers.AddQueryString(uri, "paged", "true");

            uri = QueryHelpers.AddQueryString(uri, "page", page.ToString());
            uri = QueryHelpers.AddQueryString(uri, "pageSize", pageSize.ToString());
            if (!string.IsNullOrWhiteSpace(query))
                uri = QueryHelpers.AddQueryString(uri, nameof(query), query);


            var response = await _httpClient.GetAsync(uri);

            return await response.ToResult<PagedList<UserResponse>>();
        }

        public async Task<IResult<bool>> DeleteUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result<bool>.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.DeleteUserById, userId);
            var response = await _httpClient.DeleteAsync(uri);
            return await response.ToResult<bool>();
        }

        public async Task<IResult> UpdateUserByIdAsync(string userId, JsonPatchDocument<UserRequest> request)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result<bool>.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.UpdateUserById, userId);

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json-patch+json");
            var response = await _httpClient.PatchAsync(uri, content);
            return await response.ToResult();
        }

        public async Task<IResult<UserResponse>> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result<UserResponse>.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.GetUserById, userId);

            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<UserResponse>();
        }

        public async Task<IResult<UserResponse>> GetUserByJwtAsync(string jwtToken)
        {
            if (string.IsNullOrWhiteSpace(jwtToken))
                return await Result<UserResponse>.FailAsync("JwtToken is null or empty.");

            var uri = string.Format(EndPoints.UserController.GetUserByJWT, jwtToken);

            var content = new StringContent(JsonConvert.SerializeObject(jwtToken), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<UserResponse>();
        }

        public async Task<IResult<List<RoleResponse>>> GetUserRolesByJwtAsync(string jwtToken)
        {
            if (string.IsNullOrWhiteSpace(jwtToken))
                return await Result<List<RoleResponse>>.FailAsync("JwtToken is null or empty.");

            var uri = string.Format(EndPoints.UserController.GetUserRolesByJWT, jwtToken);

            var content = new StringContent(JsonConvert.SerializeObject(jwtToken), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<List<RoleResponse>>();
        }

        public async Task<IResult<UserResponse>> GetUserByRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return await Result<UserResponse>.FailAsync("RefreshToken is null or empty.");

            var uri = string.Format(EndPoints.UserController.GetUserByRefreshToken, refreshToken);

            var content = new StringContent(JsonConvert.SerializeObject(refreshToken), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<UserResponse>();
        }

        public async Task<IResult<bool>> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result<bool>.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.ChangePassword, userId);

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<bool>();
        }

        public async Task<IResult<List<TenantResponse>>> GetTenantsByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result<List<TenantResponse>>.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.GetTenantsByUserId, userId);

            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<List<TenantResponse>>();
        }

        public async Task<IResult> UpdateUserTenantsAsync(string userId, List<string> clients)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.ReplaceUserTenants, userId);

            var content = new StringContent(JsonConvert.SerializeObject(clients), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult();
        }

        public async Task<IResult<string>> AddNewUserAsync(UserRequest request)
        {
            var uri = string.Format(EndPoints.UserController.AddNewUser);
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<string>();
        }

        public async Task<IResult<string>> AddNewUserWithPasswordAsync(NewUserRequest request)
        {
            var uri = string.Format(EndPoints.UserController.AddNewUserWithPassword);

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<string>();
        }

        public async Task<IResult<bool>> SetPasswordByuserIdAsync(string userId, SetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result<bool>.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.AuthController.SetPassword, userId);

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<bool>();
        }

        public async Task<IResult> UpdateUserRolesAsync(string userId, List<string> roles)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.ReplaceUserRoles, userId);

            var content = new StringContent(JsonConvert.SerializeObject(roles), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult();
        }

        public async Task<IResult<List<RoleResponse>>> GetUserRolesByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result<List<RoleResponse>>.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.GetUserRolesByUserIdAsync, userId);

            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<List<RoleResponse>>();
        }

        public async Task<IResult<List<ClaimResponse>>> GetClaimsByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result<List<ClaimResponse>>.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.GetClaimsByUserId, userId);
            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<List<ClaimResponse>>();
        }
        public async Task<IResult<List<UserSessionResponse>>> GetUserSessionsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result<List<UserSessionResponse>>.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.GetActiveSessions, userId);
            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<List<UserSessionResponse>>();
        }


        public async Task<IResult<List<UserClaimResponse>>> GetUserClaimsByJwtAsync(string jwtToken)
        {
            if (string.IsNullOrWhiteSpace(jwtToken))
                return await Result<List<UserClaimResponse>>.FailAsync("JwtToken is null or empty.");

            var uri = string.Format(EndPoints.UserController.GetUserClaimsByJWT, jwtToken);

            var content = new StringContent(JsonConvert.SerializeObject(jwtToken), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<List<UserClaimResponse>>();
        }

        public async Task<IResult<List<TenantResponse>>> GetUserTenantsByJwtAsync(string jwtToken)
        {
            if (string.IsNullOrWhiteSpace(jwtToken))
                return await Result<List<TenantResponse>>.FailAsync("JwtToken is null or empty.");

            var uri = string.Format(EndPoints.UserController.GetUserTenantsByJWT, jwtToken);

            var content = new StringContent(JsonConvert.SerializeObject(jwtToken), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<List<TenantResponse>>();
        }

        public async Task<IResult> UpdateUserClaimsAsync(string userId, List<UserClaimRequest> claims)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.ReplaceUserClaims, userId);

            var content = new StringContent(JsonConvert.SerializeObject(claims), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult();

        }

        public async Task<IResult<bool>> TerminateSessionAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return await Result<bool>.FailAsync("SessionId is null or empty.");

            var uri = string.Format(EndPoints.UserController.TerminateSession, sessionId);
            var response = await _httpClient.DeleteAsync(uri);
            return await response.ToResult<bool>();
        }
    }
}
