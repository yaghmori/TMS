using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Text;
using TMS.Shared.Constants;
using TMS.Shared.Extensions;
using TMS.Shared.PagedCollections;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;

namespace TMS.Core.Application.DataServices
{

    public class RoleDataService : DataServiceBase, IRoleDataService
    {
        public RoleDataService(IHttpClientFactory httpClient) : base(httpClient)
        {
        }

        public async Task<IResult<List<RoleResponse>>> GetRolesAsync(string? query = null,string?userId=null)
        {
            var uri = EndPoints.RoleController.GetRoles;
            uri = QueryHelpers.AddQueryString(uri, "paged", "false");
            if (!string.IsNullOrWhiteSpace(query))
                uri = QueryHelpers.AddQueryString(uri, nameof(query), query);

            if (!string.IsNullOrWhiteSpace(userId))
                uri = QueryHelpers.AddQueryString(uri, nameof(userId), userId);

            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<List<RoleResponse>>();
        }
        public async Task<IResult<List<ClaimResponse>>> GetClaimsByRoleIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return await Result<List<ClaimResponse>>.FailAsync("RoleId is null or empty.");

            string uri = string.Format(EndPoints.RoleController.GetRoleClaims, id);

            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<List<ClaimResponse>>();
        }

        public async Task<IResult<IPagedList<RoleResponse>>> GetRolesPagedAsync(int page = 0, int pageSize = 10, string query = null)
        {
            var uri = EndPoints.RoleController.GetRoles;

            //uri = QueryHelpers.AddQueryString(uri, "paged", "true");

            uri = QueryHelpers.AddQueryString(uri, "page", page.ToString());
            uri = QueryHelpers.AddQueryString(uri, "pageSize", pageSize.ToString());
            if (!string.IsNullOrWhiteSpace(query))
                uri = QueryHelpers.AddQueryString(uri, nameof(query), query);

            var response = await _httpClient.GetAsync(uri);

            return await response.ToResult<PagedList<RoleResponse>>();
        }

        public async Task<IResult<List<UserResponse>>> GetUsersByRoleNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return await Result<List<UserResponse>>.FailAsync("RoleName is null or empty.");

            string uri = string.Format(EndPoints.RoleController.GetUsersByRoleName, roleName);

            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<List<UserResponse>>();
        }

        public async Task<IResult<List<UserResponse>>> GetUsersByRoleIdAsync(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return await Result<List<UserResponse>>.FailAsync("RoleId is null or empty.");

            string uri = string.Format(EndPoints.RoleController.GetUsersByRoleId, id);

            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<List<UserResponse>>();
        }

        public async Task<IResult<RoleResponse>> GetRoleByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return await Result<RoleResponse>.FailAsync("RoleId is null or empty.");

            string uri = string.Format(EndPoints.RoleController.GetRoleById, id);

            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<RoleResponse>();
        }

        public async Task<IResult<RoleResponse>> GetRoleByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return await Result<RoleResponse>.FailAsync("RoleName is null or empty.");

            string uri = string.Format(EndPoints.RoleController.GetRoleByName, roleName);

            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<RoleResponse>();
        }


        public async Task<IResult<string>> AddRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return await Result<string>.FailAsync("RoleName is null or empty.");

            string uri = string.Format(EndPoints.RoleController.AddRole, roleName);


            var content = new StringContent(JsonConvert.SerializeObject(roleName), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<string>();
        }

        public async Task<IResult<bool>> DeleteRoleByIdAsync(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return await Result<bool>.FailAsync("RoleId is null or empty.");

            string uri = string.Format(EndPoints.RoleController.DeleteRoleById, id);


            var response = await _httpClient.DeleteAsync(uri);
            return await response.ToResult<bool>();
        }

        public async Task<IResult<bool>> DeleteRoleByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return await Result<bool>.FailAsync("RoleName is null or empty.");

            string uri = string.Format(EndPoints.RoleController.DeleteRoleByName, roleName);


            var response = await _httpClient.DeleteAsync(uri);
            return await response.ToResult<bool>();
        }

        public async Task<IResult<bool>> UpdateRoleByIdAsync(string id, string roleName)
        {
            if (string.IsNullOrWhiteSpace(id))
                return await Result<bool>.FailAsync("RoleId is null or empty.");

            var uri = string.Format(EndPoints.RoleController.UpdateRoleById, id);
            uri = QueryHelpers.AddQueryString(uri, nameof(roleName), roleName);

            var response = await _httpClient.PatchAsync(uri, null);
            return await response.ToResult<bool>();
        }

        public async Task<IResult<List<RoleResponse>>> GetRolesByUserIdAsync(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
                return await Result<List<RoleResponse>>.FailAsync("RoleId is null or empty.");

            string uri = string.Format(EndPoints.RoleController.GetRolesByUserId, id);

            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<List<RoleResponse>>();
        }

        public async Task<IResult<bool>> UpdateUserRolesAsync(string userId, List<string> roles)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result<bool>.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserController.ReplaceUserRoles, userId);

            var content = new StringContent(JsonConvert.SerializeObject(roles), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<bool>();
        }

        public async Task<IResult<bool>> UpdateRoleUsersAsync(string roleId, List<string> users)
        {

            if (string.IsNullOrWhiteSpace(roleId))
                return await Result<bool>.FailAsync("RoleId is null or empty.");

            var uri = string.Format(EndPoints.RoleController.ReplaceRoleUsers, roleId);

            var content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<bool>();
        }

        public async Task<IResult> UpdateRoleClaimsAsync(string roleId, List<ClaimResponse> claims)
        {

            if (string.IsNullOrWhiteSpace(roleId))
                return await Result.FailAsync("RoleId is null or empty.");

            var uri = string.Format(EndPoints.RoleController.UpdateRoleClaims, roleId);

            var content = new StringContent(JsonConvert.SerializeObject(claims), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult();
        }

    }
}
