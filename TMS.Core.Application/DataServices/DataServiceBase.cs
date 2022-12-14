using Newtonsoft.Json;
using TMS.Shared.Constants;

namespace TMS.Core.Application.DataServices
{
    public class DataServiceBase
    {
        public readonly HttpClient _httpClient;

        public DataServiceBase(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(ApplicationConstants.ServerHttpClientName);

        }
        public static Uri BuildUri(string path, Dictionary<string, string> queryParams)
        {
            var uriBuilder = new UriBuilder() { Path = path };
            if (queryParams != null)
            {
                var query = string.Join("&", queryParams.Select(x => $"{x.Key}={x.Value}").ToArray());
                uriBuilder.Query = query;
            }

            return uriBuilder.Uri;
        }
        public static async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response, T defaultValue = default)
        {
            var returnResponse = defaultValue;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                returnResponse = JsonConvert.DeserializeObject<T>(responseContent);

            }
            return returnResponse;
        }

    }
}