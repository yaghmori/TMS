using Microsoft.AspNetCore.JsonPatch;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;

namespace TMS.Core.Application.DataServices
{
    public interface IUserSettingDataService
    {
        Task<IResult<List<UserSettingsResponse>>> GetUserSettingsAsync(string? query = null);
        Task<IResult<UserSettingsResponse>> GetUserSettingsByIdAsync(string userSettingId);
        Task<IResult<bool>> DeleteUserSettingsByIdAsync(string userSettingId);
        Task<IResult<string>> AddUserSettingsAsync(UserSettingsRequest request);
        Task<IResult> UpdateSettingsAsync(string settingId, JsonPatchDocument<UserSettingsRequest> request);
        Task<IResult<UserSettingsResponse>> GetUserSettingsByUserIdAsync(string userId);
    }
}