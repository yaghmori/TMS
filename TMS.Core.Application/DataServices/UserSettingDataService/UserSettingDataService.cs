using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Text;
using TMS.Shared.Constants;
using TMS.Shared.Extensions;
using TMS.Shared.Requests;
using TMS.Shared.Responses;
using TMS.Shared.ResultWrapper;

namespace TMS.Core.Application.DataServices
{


    public class UserSettingDataService : DataServiceBase, IUserSettingDataService
    {

        public UserSettingDataService(IHttpClientFactory httpClient) : base(httpClient)
        {
        }



        public async Task<IResult<bool>> DeleteUserSettingsByIdAsync(string usersettingId)
        {

            if (string.IsNullOrWhiteSpace(usersettingId))
                return await Result<bool>.FailAsync("UsersettingId is null or empty.");

            var uri = string.Format(EndPoints.UserSettingController.DeleteUserSettingById, usersettingId);

            var response = await _httpClient.DeleteAsync(uri);
            return await response.ToResult<bool>();
        }

        public async Task<IResult<UserSettingsResponse>> GetUserSettingsByIdAsync(string usersettingId)
        {
            if (string.IsNullOrWhiteSpace(usersettingId))
                return await Result<UserSettingsResponse>.FailAsync("UsersettingId is null or empty.");


            var uri = string.Format(EndPoints.UserSettingController.GetUserSettingById, usersettingId);

            var response = await _httpClient.GetAsync(uri);
            return await response.ToResult<UserSettingsResponse>();
        }

        public async Task<IResult<List<UserSettingsResponse>>> GetUserSettingsAsync(string? query = null)
        {
            var uri = EndPoints.UserSettingController.GetUserSettings;

            if (!string.IsNullOrWhiteSpace(query))
                uri = QueryHelpers.AddQueryString(uri, nameof(query), query);

            var response = await _httpClient.GetAsync(uri);

            return await response.ToResult<List<UserSettingsResponse>>();
        }

        public async Task<IResult<UserSettingsResponse>> GetUserSettingsByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return await Result<UserSettingsResponse>.FailAsync("UserId is null or empty.");

            var uri = string.Format(EndPoints.UserSettingController.GetUserSettingByUserId,userId);
            var response = await _httpClient.GetAsync(uri);

            return await response.ToResult<UserSettingsResponse>();
        }

        public async Task<IResult<string>> AddUserSettingsAsync(UserSettingsRequest request)
        {
            var uri = EndPoints.UserSettingController.AddUserSetting;

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            return await response.ToResult<string>();
        }

        public async Task<IResult> UpdateSettingsAsync(string userSettingId, JsonPatchDocument<UserSettingsRequest> request)
        {
            if (string.IsNullOrWhiteSpace(userSettingId))
                return await Result<bool>.FailAsync("UserSettingId is null or empty.");

            var uri = string.Format(EndPoints.UserSettingController.UpdateUserSettingById, userSettingId);

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json-patch+json");
            var response = await _httpClient.PatchAsync(uri, content);
            return await response.ToResult();
        }


    }
}
