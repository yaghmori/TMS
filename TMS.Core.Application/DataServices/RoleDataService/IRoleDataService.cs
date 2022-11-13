using TMS.Shared.PagedCollections;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;

namespace TMS.Core.Application.DataServices
{
    public interface IRoleDataService
    {
        Task<IResult<string>> AddRoleAsync(string roleName);
        Task<IResult<bool>> DeleteRoleByIdAsync(string id);
        Task<IResult<bool>> DeleteRoleByNameAsync(string roleName);
        Task<IResult<RoleResponse>> GetRoleByIdAsync(string id);
        Task<IResult<RoleResponse>> GetRoleByNameAsync(string roleName);
        Task<IResult<List<RoleResponse>>> GetRolesAsync(string? query = null, string? userId = null);
        Task<IResult<List<ClaimResponse>>> GetClaimsByRoleIdAsync(string id);
        Task<IResult<List<RoleResponse>>> GetRolesByUserIdAsync(string id);
        Task<IResult<IPagedList<RoleResponse>>> GetRolesPagedAsync(int page = 0, int pageSize = 10, string query = null);
        Task<IResult<List<UserResponse>>> GetUsersByRoleIdAsync(string id);
        Task<IResult<List<UserResponse>>> GetUsersByRoleNameAsync(string roleName);
        Task<IResult<bool>> UpdateRoleUsersAsync(string roleId, List<string> users);
        Task<IResult<bool>> UpdateUserRolesAsync(string userId, List<string> roles);
        Task<IResult<bool>> UpdateRoleByIdAsync(string id, string roleName);
        Task<IResult> UpdateRoleClaimsAsync(string roleId, List<ClaimResponse> claims);
    }
}