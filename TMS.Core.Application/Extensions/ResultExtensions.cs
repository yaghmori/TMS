using Newtonsoft.Json;
using TMS.Shared.ResultWrapper;

namespace TMS.Shared.Extensions
{
    public static class ResultExtensions
    {
        internal static async Task<IResult<T>> ToResult<T>(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Result<T>>(responseAsString);
            return responseObject;
        }
        internal static async Task<T> ToApiResult<T>(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<T>(responseAsString);
            return responseObject;
        }



        internal static async Task<IResult> ToResult(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<Result<IResult>>(responseAsString);
            return responseObject;
        }

        internal static async Task<PaginatedResult<T>> ToPaginatedResult<T>(this HttpResponseMessage response)
        {
            var responseAsString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<PaginatedResult<T>>(responseAsString);
            return responseObject;
        }
    }
}
