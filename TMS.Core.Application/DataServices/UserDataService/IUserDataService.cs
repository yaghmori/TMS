using Microsoft.AspNetCore.JsonPatch;
using TMS.Shared.PagedCollections;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;

namespace TMS.Core.Application.DataServices
{
    public interface IUserDataService
    {
        Task<IResult<string>> AddNewUserAsync(UserRequest request);
        Task<IResult<string>> AddNewUserWithPasswordAsync(NewUserRequest request);
        Task<IResult<bool>> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<IResult<bool>> DeleteUserByIdAsync(string userId);
        Task<IResult<bool>> TerminateSessionAsync(string sessionId);
        Task<IResult<List<TenantResponse>>> GetTenantsByUserIdAsync(string userId);
        Task<IResult<List<RoleResponse>>> GetUserRolesByUserIdAsync(string userId);
        Task<IResult<List<RoleResponse>>> GetUserRolesByJwtAsync(string jwtToken);
        Task<IResult<UserResponse>> GetUserByIdAsync(string userId);
        Task<IResult<UserResponse>> GetUserByJwtAsync(string jwtToken);
        Task<IResult<List<ClaimResponse>>> GetClaimsByUserIdAsync(string userId);
        Task<IResult<List<UserClaimResponse>>> GetUserClaimsByJwtAsync(string jwtToken);
        Task<IResult<List<TenantResponse>>> GetUserTenantsByJwtAsync(string jwtToken);
        Task<IResult<UserResponse>> GetUserByRefreshTokenAsync(string refreshToken);
        Task<IResult<List<UserResponse>>> GetUsersAsync(string? query = null);
        Task<IResult<IPagedList<UserResponse>>> GetUsersPagedAsync(int page = 0, int pageSize = 10, string query = null);
        Task<IResult> UpdateUserTenantsAsync(string userId, List<string> clients);
        Task<IResult> UpdateUserClaimsAsync(string userId, List<UserClaimRequest> claims);
        Task<IResult> UpdateUserRolesAsync(string userId, List<string> roles);
        Task<IResult<bool>> SetPasswordByuserIdAsync(string userId, SetPasswordRequest request);
        Task<IResult> UpdateUserByIdAsync(string userId, JsonPatchDocument<UserRequest> request);
        Task<IResult<List<UserSessionResponse>>> GetUserSessionsAsync(string userId);
    }
}