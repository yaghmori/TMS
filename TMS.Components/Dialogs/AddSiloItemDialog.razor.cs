using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.JsonPatch;
using MudBlazor;
using TMS.Core.Domain.Enums;
using TMS.Shared.Requests;

namespace TMS.RootComponents.Dialogs
{
    public partial class AddSiloItemDialog
    {
        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }
        [Parameter] public string ParentId { get; set; } = null;
        [Parameter] public string SiloItemId { get; set; } = string.Empty;
        [Parameter] public SiloItemTypeEnum ItemType { get; set; } = SiloItemTypeEnum.Silo;

        private List<SiloItemRequest> Ancestors = new();
        private EditForm editForm ;
        public SiloItemRequest SiloItem { get; set; } = new();
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        public HashSet<SiloItemTypeEnum> SiloItemTypeValues { get; set; } = Enum.GetValues<SiloItemTypeEnum>().ToHashSet();
        public HashSet<WarningServiceTypeEnum> WarningServiceTypeValues { get; set; } = Enum.GetValues<WarningServiceTypeEnum>().ToHashSet();
        protected async override Task OnInitializedAsync()
        {
            IsLoading = true;
            if (string.IsNullOrWhiteSpace(SiloItemId)) //New
            {
                var lastIndex = await _siloItemDataService.GetLastIndexAsync(ItemType, ParentId);
                var lastAddress = await _siloItemDataService.GetLastAddressAsync(ItemType, ParentId);
                SiloItem = new SiloItemRequest();
                SiloItem.ParentId = string.IsNullOrWhiteSpace(ParentId) ? null : Guid.Parse(ParentId);
                SiloItem.ItemType = ItemType;
                SiloItem.Index = lastIndex + 1;
                SiloItem.Address = lastAddress + 1;
                editForm?.EditContext?.NotifyValidationStateChanged();
                editForm?.EditContext?.MarkAsUnmodified();
            }
            else
            {
                var siloItem = await _siloItemDataService.GetSiloItemByIdAsync(SiloItemId);
                SiloItem = new();
                SiloItem = _autoMapper.Map<SiloItemRequest>(siloItem);
                ParentId = SiloItem?.ParentId.ToString();
            }

            var parent = string.IsNullOrWhiteSpace(ParentId) ? null : await _siloItemDataService.GetSiloItemByIdAsync(ParentId);
            var ancestors = await _siloItemDataService.GetAncestorsByIdAsync(ParentId);
            switch (SiloItem.ItemType)
            {
                case SiloItemTypeEnum.Silo:
                    Ancestors.Add(SiloItem);
                    break;
                case SiloItemTypeEnum.Loop:
                    Ancestors.Add(_autoMapper.Map<SiloItemRequest>(parent));
                    Ancestors.Add(SiloItem);
                    break;
                case SiloItemTypeEnum.Cable:
                    foreach (var item in ancestors)
                    {
                        Ancestors.Add(_autoMapper.Map<SiloItemRequest>(item));
                    }

                    Ancestors.Add(_autoMapper.Map<SiloItemRequest>(parent));
                    Ancestors.Add(_autoMapper.Map<SiloItemRequest>(SiloItem));
                    break;
                case SiloItemTypeEnum.TempSensor:
                    if (SiloItem.Feature == SensorFeatureEnum.None)
                    {
                        foreach (var item in ancestors)
                        {
                            Ancestors.Add(_autoMapper.Map<SiloItemRequest>(item));
                        }

                        Ancestors.Add(_autoMapper.Map<SiloItemRequest>(parent));
                        Ancestors.Add(_autoMapper.Map<SiloItemRequest>(SiloItem));
                    }
                    else
                    {
                        Ancestors.Add(_autoMapper.Map<SiloItemRequest>(parent));
                        Ancestors.Add(_autoMapper.Map<SiloItemRequest>(SiloItem));
                    }

                    break;
                case SiloItemTypeEnum.HumiditySensor:
                    Ancestors.Add(_autoMapper.Map<SiloItemRequest>(parent));
                    Ancestors.Add(_autoMapper.Map<SiloItemRequest>(SiloItem));
                    break;
                default:
                    break;
            }

            IsLoading = false;
            SiloItem.IsReadOnly = false;
        }

        async void SaveChangesSubmit(EditContext context)
        {
            IsSaving = true;
            bool result = false;
            if (string.IsNullOrWhiteSpace(SiloItemId)) //New
            {
                var ids = await _siloItemDataService.AddSiloItemAsync(SiloItem);
                if (ids.Count > 0)
                {
                    result = true;
                    _snackbar.Add(_messageLocalizer["SiloItemsSuccessfullyAdded", ids.Count].Value, Severity.Success);
                }
                else
                {
                    _snackbar.Add(_messageLocalizer["CreatingSiloItemFailed"].Value, Severity.Error);
                }
            }
            else //Edit
            {
                var patchDoc = new JsonPatchDocument<SiloItemRequest>();
                patchDoc.Replace(e => e.Index, SiloItem.Index);
                patchDoc.Replace(e => e.Name, SiloItem.Name);
                if (SiloItem.ItemType == SiloItemTypeEnum.HumiditySensor || SiloItem.ItemType == SiloItemTypeEnum.TempSensor)
                {
                    patchDoc.Replace(e => e.Address, SiloItem.Address);
                    patchDoc.Replace(e => e.Rom, SiloItem.Rom);
                    patchDoc.Replace(e => e.Box, SiloItem.Box);
                    patchDoc.Replace(e => e.Line, SiloItem.Line);
                    patchDoc.Replace(e => e.FalseValueCount, SiloItem.FalseValueCount);
                }

                var updateResult = await _siloItemDataService.UpdateSiloItemByIdAsync(SiloItemId, patchDoc);
                if (updateResult)
                {
                    result = true;
                    _snackbar.Add(_messageLocalizer["SiloItemSuccessfullyUpdated"].Value, Severity.Success);
                }
                else
                    _snackbar.Add(_messageLocalizer["UpdatingSiloItemFailed"].Value, Severity.Error);
            }
            IsSaving = false;
            MudDialog.Close(DialogResult.Ok(result));
        }
    }
}