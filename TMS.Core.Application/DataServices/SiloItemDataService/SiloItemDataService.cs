using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System.Text;
using TMS.Core.Domain.Enums;
using TMS.Shared.Constants;
using TMS.Shared.Extensions;
using TMS.Shared.PagedCollections;
using TMS.Shared.Requests;
using TMS.Shared.Responses;

namespace TMS.Core.Application.DataServices
{
    public class SiloItemDataService : DataServiceBase, ISiloItemDataService
    {

        public SiloItemDataService(IHttpClientFactory httpClient) : base(httpClient)
        {
        }

        public async Task<List<SiloItemResponse>> GetSiloItemsAsync()
        {
            var uri = EndPoints.SiloItemController.GetAllSiloItems;

            List<SiloItemResponse> result = new List<SiloItemResponse>();
            var response = await _httpClient.GetAsync(uri);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.ToApiResult<List<SiloItemResponse>>();
            }
            return result;
        }

        public async Task<SiloItemResponse> GetChildByIdAsync(string id)
        {
            SiloItemResponse result = null;

            if (string.IsNullOrWhiteSpace(id))
                return result;

            var uri = string.Format(EndPoints.SiloItemController.GetChildById, id);

            var response = await _httpClient.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.ToApiResult<SiloItemResponse>();
            }
            return result;
        }

        public async Task<SiloItemResponse> GetSiloItemByIdAsync(string id)
        {
            SiloItemResponse result = null;

            if (string.IsNullOrWhiteSpace(id))
                return result;

            var uri = string.Format(EndPoints.SiloItemController.GetSiloItemById, id);

            var response = await _httpClient.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.ToApiResult<SiloItemResponse>();
            }
            return result;
        }

        public async Task<List<SiloItemResponse>> GetByItemTypeAsync(SiloItemTypeEnum type)
        {
            List<SiloItemResponse> result = new List<SiloItemResponse>();

            var uri = EndPoints.SiloItemController.GetByItemType;
            uri = QueryHelpers.AddQueryString(uri, nameof(type), type.ToString());

            var response = await _httpClient.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.ToApiResult<List<SiloItemResponse>>();
            }
            return result;
        }


        public async Task<SiloItemResponse> GetParentByIdAsync(string id)
        {
            SiloItemResponse result = null;

            if (string.IsNullOrWhiteSpace(id))
                return result;

            var uri = string.Format(EndPoints.SiloItemController.GetParentById, id);

            var response = await _httpClient.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.ToApiResult<SiloItemResponse>();
            }
            return result;
        }

        public async Task<bool> DeleteSiloItemByIdAsync(string id)
        {
            bool result = false;

            if (string.IsNullOrWhiteSpace(id))
                return result;

            var uri = string.Format(EndPoints.SiloItemController.DeleteSiloItemById, id);

            var response = await _httpClient.DeleteAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = true;
            }
            return result;
        }

        public async Task<bool> UpdateSiloItemByIdAsync(string id, JsonPatchDocument<SiloItemRequest> request)
        {
            bool result = false;

            if (string.IsNullOrWhiteSpace(id))
                return result;

            var uri = string.Format(EndPoints.SiloItemController.UpdateSiloItemById, id);

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json-patch+json");
            var response = await _httpClient.PatchAsync(uri, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = true;
            }

            return result;
        }

        public async Task<List<string>> AddSiloItemAsync(SiloItemRequest request)
        {
            var uri = string.Format(EndPoints.SiloItemController.AddSiloItem);
            List<string> result = new();

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(uri, content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.ToApiResult<List<string>>();
            }

            return result;
        }

        public async Task<bool> DeleteAllSiloItemsAsync()
        {
            var uri = string.Format(EndPoints.SiloItemController.DeleteAllSiloItems);

            bool result = false;
            var response = await _httpClient.DeleteAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                result = true;
            }
            return result;
        }

        public async Task<int> GetLastIndexAsync(SiloItemTypeEnum type, string? parentId = null)
        {
            var uri = EndPoints.SiloItemController.GetLastIndex;
            uri = QueryHelpers.AddQueryString(uri, nameof(type), type.ToString());
            if (!string.IsNullOrWhiteSpace(parentId))
                uri = QueryHelpers.AddQueryString(uri, nameof(parentId), parentId);

            int result = 0;
            var response = await _httpClient.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.ToApiResult<int>();
            }
            return result;
        }

        public async Task<int> GetLastAddressAsync(SiloItemTypeEnum type, string? parentId = null)
        {
            var uri = EndPoints.SiloItemController.GetLastAddress;
            uri = QueryHelpers.AddQueryString(uri, nameof(type), type.ToString());
            if (!string.IsNullOrWhiteSpace(parentId))
                uri = QueryHelpers.AddQueryString(uri, nameof(parentId), parentId);
            int result = 0;
            var response = await _httpClient.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.ToApiResult<int>();
            }
            return result;
        }

        public async Task<List<SiloItemResponse>> GetAncestorsByIdAsync(string id)
        {
            List<SiloItemResponse> result = new();

            if (string.IsNullOrWhiteSpace(id))
                return result;

            var uri = string.Format(EndPoints.SiloItemController.GetAncestorsById, id);
            var response = await _httpClient.GetAsync(uri);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.ToApiResult<List<SiloItemResponse>>();
            }
            return result;
        }

        public async Task<IPagedList<SensorHistoryResponse>> GetPagedHistoriesAsync(string id, int page = 0, int pageSize = 10, DateTime? startDate = null, DateTime? endDate = null, string query = null)
        {
            PagedList<SensorHistoryResponse> result = new PagedList<SensorHistoryResponse>();

            if (string.IsNullOrWhiteSpace(id))
                return result;

            var uri = string.Format(EndPoints.SiloItemController.GetHistoriesById, id);
            uri = QueryHelpers.AddQueryString(uri, "page", page.ToString());
            uri = QueryHelpers.AddQueryString(uri, "pageSize", pageSize.ToString());
            if (!string.IsNullOrWhiteSpace(query))
                uri = QueryHelpers.AddQueryString(uri, nameof(query), query);
            if (startDate != null)
                uri = QueryHelpers.AddQueryString(uri, nameof(startDate), startDate.ToString());
            if (endDate != null)
                uri = QueryHelpers.AddQueryString(uri, nameof(endDate), endDate.ToString());


            var response = await _httpClient.GetAsync(uri);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await response.ToApiResult<PagedList<SensorHistoryResponse>>();
            }
            return result;
        }


    }
}
