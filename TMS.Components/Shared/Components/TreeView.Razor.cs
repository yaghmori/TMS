using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.JsonPatch;
using MudBlazor;
using TMS.Core.Domain.Enums;
using TMS.RootComponents.Dialogs;
using TMS.Shared.Requests;
using TMS.Shared.Responses;

namespace TMS.RootComponents.Shared.Components
{
    public partial class TreeView 
    {
        [Parameter]
        public EventCallback<SiloItemResponse> SelectedItemChanged { get; set; }
        [Parameter]
        public SiloItemResponse SelectedSiloItem { get; set; } = new();

        public string SiloItemId { get; set; } = string.Empty;
        public List<SiloItemResponse> SiloItems { get; set; } = new();

        public async Task<HashSet<SiloItemResponse>> LoadServerData(SiloItemResponse parentNode)
        {
            var items = await _siloItemDataService.GetAncestorsByIdAsync(parentNode.Id);
            return items.ToHashSet();
        }

        public async Task ReloadData()
        {
            IsLoading = true;
            SiloItems = await _siloItemDataService.GetSiloItemsAsync();
            IsLoading = false;

        }

        protected override async Task OnInitializedAsync()
        {
            await ReloadData();
        }

        public async Task AddSiloItem(SiloItemResponse parent, SiloItemTypeEnum type)
        {
            var parameters = new DialogParameters
            {
                { "ParentId", parent != null ? parent.Id : string.Empty },
                { "ItemType", type }
            };

            var title = _viewLocalizer["Add"].Value + " " + type.ToString();

            var options = new DialogOptions()
            { CloseButton = true, DisableBackdropClick = true, FullWidth = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = _dialog.Show<AddSiloItemDialog>(title, parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await ReloadData();
            }
        }

        public async Task OpenSiloItemInfo(SiloItemResponse siloItem)
        {
            if (siloItem != null)
            {
                var parameters = new DialogParameters
                {{ "SiloItemId", siloItem.Id }};
                var options = new DialogOptions()
                { CloseButton = true, DisableBackdropClick = true, FullWidth = true, MaxWidth = MaxWidth.ExtraSmall };
                var dialog = _dialog.Show<AddSiloItemDialog>(siloItem.DisplayMember, parameters, options);
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    await ReloadData();
                }
            }
        }

        public async Task DeleteSiloItem(SiloItemResponse item)
        {
            var parameters = new DialogParameters
            {
                { "Title", item.DisplayMember },
                { "ButtonText", _viewLocalizer["Delete"].Value },
                { "ContentText", _messageLocalizer["DeleteSiloItemConfirmation", item.ItemType].Value },
                { "ButtonColor", Color.Error },
                { "ButtonIcon", Icons.Rounded.Delete },
                { "TitleIcon", Icons.Rounded.Delete },
            };
            var options = new DialogOptions()
            {
                CloseButton = true,
                MaxWidth = MaxWidth.ExtraSmall
            };
            var dialog = _dialog.Show<MessageDialog>("", parameters, options);
            var dgResult = await dialog.Result;
            if (!dgResult.Cancelled)
            {
                var result = await _siloItemDataService.DeleteSiloItemByIdAsync(item.Id);
                if (result)
                {
                    if (SelectedSiloItem.Id == item.Id)
                        SelectedSiloItem = null;
                    await SelectedItemChanged.InvokeAsync(SelectedSiloItem);

                    _snackbar.Add(_messageLocalizer["DeleteSiloItemSuccessfull"].Value, Severity.Success);
                    await ReloadData();
                }
                else
                    _snackbar.Add(_messageLocalizer["DeleteSiloItemFailed"].Value, Severity.Error);

            }
        }

        public async Task Calibration(SiloItemResponse item)
        {
            var parameters = new DialogParameters();
            var title = "";
            if (item != null)
            {
                parameters.Add("SiloItemId", item.Id.ToString());
                title = _viewLocalizer["UpdateSiloItem"].Value;
            }
            else
            {
                parameters.Add("SiloItemId", string.Empty);
                parameters.Add("ParentId", item != null ? item?.Id : null);
                title = _viewLocalizer["AddItem"].Value;
            }

            var options = new DialogOptions()
            { CloseButton = true, DisableBackdropClick = true, FullWidth = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = _dialog.Show<AddSiloItemDialog>(title, parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await ReloadData();
            }
        }

        public async Task ResetHistories(SiloItemResponse item)
        {
            var parameters = new DialogParameters();
            var title = "";
            if (item != null)
            {
                parameters.Add("SiloItemId", item.Id.ToString());
                title = _viewLocalizer["UpdateSiloItem"].Value;
            }
            else
            {
                parameters.Add("SiloItemId", string.Empty);
                parameters.Add("ParentId", item != null ? item?.Id : null);
                title = _viewLocalizer["AddItem"].Value;
            }

            var options = new DialogOptions()
            { CloseButton = true, DisableBackdropClick = true, FullWidth = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = _dialog.Show<AddSiloItemDialog>(title, parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await ReloadData();
            }
        }

        public async Task ResetFalseValueCounter(SiloItemResponse item)
        {
            var parameters = new DialogParameters();
            var title = "";
            if (item != null)
            {
                parameters.Add("SiloItemId", item.Id.ToString());
                title = _viewLocalizer["UpdateSiloItem"].Value;
            }
            else
            {
                parameters.Add("SiloItemId", string.Empty);
                parameters.Add("ParentId", item != null ? item?.Id : null);
                title = _viewLocalizer["AddItem"].Value;
            }

            var options = new DialogOptions()
            { CloseButton = true, DisableBackdropClick = true, FullWidth = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = _dialog.Show<AddSiloItemDialog>(title, parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                await ReloadData();
            }
        }

        public async Task DeleteAllSiloItem()
        {
            var parameters = new DialogParameters();
            parameters.Add("ButtonText", _viewLocalizer["DeleteAllSiloItems"].Value);
            parameters.Add("ContentText", _messageLocalizer["DeleteAllSiloItemConfirmation"].Value);
            parameters.Add("ButtonColor", Color.Error);
            var options = new DialogOptions()
            { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
            var dialog = _dialog.Show<MessageDialog>(_viewLocalizer["DeleteAllSiloItems"].Value, parameters, options);
            var dgResult = await dialog.Result;
            if (!dgResult.Cancelled)
            {
                var result = await _siloItemDataService.DeleteAllSiloItemsAsync();
                if (result)
                {
                    SelectedSiloItem = null;
                    await SelectedItemChanged.InvokeAsync(SelectedSiloItem);

                    _snackbar.Add(_messageLocalizer["DeleteAllSiloItemsCompletedSuccessfull"].Value, Severity.Success);
                    await ReloadData();
                }
                else
                    _snackbar.Add(_messageLocalizer["DeleteAllSiloItemsFailed"].Value, Severity.Error);

            }
        }

        public async Task ImportSensorData()
        {
        }

        public async Task ExportSensorData()
        {
        }

        public async Task<bool> OnExpanding(SiloItemResponse siloItem)
        {
            siloItem.IsExpanded = !siloItem.IsExpanded;
            var patchDoc = new JsonPatchDocument<SiloItemRequest>();
            patchDoc.Replace(e => e.IsExpanded, siloItem.IsExpanded);

            var result = await _siloItemDataService.UpdateSiloItemByIdAsync(siloItem.Id, patchDoc);
            return siloItem.IsExpanded;
        }

    }
}