using Microsoft.AspNetCore.JsonPatch;
using TMS.Core.Domain.Enums;
using TMS.Shared.PagedCollections;
using TMS.Shared.Requests;
using TMS.Shared.Responses;

namespace TMS.Core.Application.DataServices
{
    public interface ISiloItemDataService
    {
        Task<List<SiloItemResponse>> GetSiloItemsAsync();
        Task<SiloItemResponse> GetChildByIdAsync(string id);
        Task<List<SiloItemResponse>> GetByItemTypeAsync(SiloItemTypeEnum type);
        Task<SiloItemResponse> GetSiloItemByIdAsync(string id);
        Task<SiloItemResponse> GetParentByIdAsync(string id);
        Task<bool> DeleteSiloItemByIdAsync(string id);
        Task<bool> DeleteAllSiloItemsAsync();

        Task<bool> UpdateSiloItemByIdAsync(string id, JsonPatchDocument<SiloItemRequest> request);
        Task<List<string>> AddSiloItemAsync(SiloItemRequest request);
        Task<int> GetLastIndexAsync(SiloItemTypeEnum type, string? parentId = null);
        Task<int> GetLastAddressAsync(SiloItemTypeEnum type, string? parentId = null);
        Task<List<SiloItemResponse>> GetAncestorsByIdAsync(string id);
        Task<IPagedList<SensorHistoryResponse>> GetPagedHistoriesAsync(string id, int page = 0, int pageSize = 10,DateTime? startDate=null , DateTime? endDate=null, string query = null);
    }
}