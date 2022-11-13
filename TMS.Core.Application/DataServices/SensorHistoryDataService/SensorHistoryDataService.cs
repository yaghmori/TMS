using Microsoft.AspNetCore.WebUtilities;
using TMS.Core.Application.DataServices;
using TMS.Core.Domain.Entities;
using TMS.Shared.Constants;
using TMS.Shared.Extensions;
using TMS.Shared.PagedCollections;

namespace TMS.Shared.DataServices
{
    public class SensorHistoryDataService : DataServiceBase, ISensorHistoryDataService
    {

        public SensorHistoryDataService(IHttpClientFactory httpClient) : base(httpClient)
        {
        }
        public async Task<IPagedList<SensorHistory>> GetHistoriesPagedAsync(int page = 0, int pageSize = 10, string query = null)
        {
            var uri = EndPoints.SiloItemController.GetHistories;

            //uri = QueryHelpers.AddQueryString(uri, "paged", "true");

            uri = QueryHelpers.AddQueryString(uri, "page", page.ToString());
            uri = QueryHelpers.AddQueryString(uri, "pageSize", pageSize.ToString());
            if (!string.IsNullOrWhiteSpace(query))
                uri = QueryHelpers.AddQueryString(uri, nameof(query), query);


            PagedList<SensorHistory> result = new PagedList<SensorHistory>();
            var response = await _httpClient.GetAsync(uri);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.ToApiResult<PagedList<SensorHistory>>();
            }
            return result;
        }




    }
}
