using Microsoft.AspNetCore.Components;
using MudBlazor;
using TMS.Shared.PagedCollections;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Shared.Components
{
    public partial class SensorHistoryView
    {
        [Parameter]
        public string SiloItemId { get; set; } = string.Empty;
        private IPagedList<SensorHistoryResponse> HistoryPagedCollection = new PagedList<SensorHistoryResponse>();
        private MudDataGrid<SensorHistoryResponse>? _mudDataGrid;
        private string Query { get; set; } = string.Empty;

        void OnSearchHistory(string query)
        {
            Query = query;
            _mudDataGrid.ReloadServerData();
        }


        private async Task<GridData<SensorHistoryResponse>> ReloadHistoriesAsync(GridState<SensorHistoryResponse> state)
        {
            if (!string.IsNullOrWhiteSpace(SiloItemId))
            {
                HistoryPagedCollection = await _siloItemDataService.GetPagedHistoriesAsync(SiloItemId, state.Page, state.PageSize, query: Query);
            }
            return new GridData<SensorHistoryResponse>
            {
                Items = HistoryPagedCollection.Items,
                TotalItems = HistoryPagedCollection.TotalCount
            };
        }

    }
}