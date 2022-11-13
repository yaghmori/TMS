using Microsoft.AspNetCore.JsonPatch;
using TMS.Shared.PagedCollections;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;

namespace TMS.Core.Application.DataServices
{
    public interface ITenantDataService
    {
        Task<IResult<string>> AddTenantAsync(TenantRequest request);
        Task<IResult<string>> AddUserToTenantAsync(string tenantId, string userId);
        Task<IResult<bool>> CreateDatabaseAsync(string tenantId);
        Task<IResult> DeleteTenantByIdAsync(string tenantId);
        Task<IResult> DeleteDatabseAsync(string tenantId);
        Task<IResult<bool>> DeleteUserTenantByIdAsync(string tenantId);
        Task<IResult<TenantResponse>> GetTenantByIdAsync(string tenantId);
        Task<IResult<List<TenantResponse>>> GetTenantsAsync(string? query = null);
        Task<IResult<IPagedList<TenantResponse>>> GetTenantsPagedAsync(int page = 0, int pageSize = 10, string query = null);
        Task<IResult<List<UserResponse>>> GetUsersByTenantId(string tenantId);
        Task<IResult<bool>> MigrateDatabseAsync(string tenantId);
        Task<IResult<bool>> RemoveUserFromTenantAsync(string tenantId, string userId);
        Task<IResult<bool>> ReplaceTenantUsersAsync(string tenantId, List<string> users);
        Task<IResult> UpdateTenantByIdAsync(string tenantId, JsonPatchDocument<TenantRequest> request);

    }
}