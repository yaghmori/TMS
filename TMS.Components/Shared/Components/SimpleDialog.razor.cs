using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace TMS.RootComponents.Shared.Components
{
    public partial class SimpleDialog
    {
        [CascadingParameter]
        MudDialogInstance MudDialog { get; set; }

        [Parameter]
        public string ContentText { get; set; }

        [Parameter]
        public string ButtonText { get; set; }

        [Parameter]
        public Color Color { get; set; }

        void Submit() => MudDialog.Close(DialogResult.Ok(true));
        void Cancel() => MudDialog.Cancel();
    }
}