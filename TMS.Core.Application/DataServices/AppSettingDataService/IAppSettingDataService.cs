using Microsoft.AspNetCore.JsonPatch;
using TMS.Shared.PagedCollections;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;

namespace TMS.Core.Application.DataServices
{
    public interface IAppSettingDataService
    {
        Task<IResult<List<AppSettingsResponse>>> GetAppSettingsAsync(string? query = null);
        Task<IResult<AppSettingsResponse>> GetAppSettingsByIdAsync(string appSettingId);
        Task<IResult<AppSettingsResponse>> GetAppSettingsByKeyAsync(string key);
        Task<IResult<bool>> DeleteAppSettingsByIdAsync(string appSettingId);
        Task<IResult<bool>> DeleteAppSettingsByKeyAsync(string key);
        Task<IResult<string>> AddAppSettingsAsync(AppSettingsRequest request);
        Task<IResult> UpdateAppSettingsByIdAsync(string appSettingId, JsonPatchDocument<AppSettingsRequest> request);

    }
}