using Microsoft.Extensions.Localization;
using TMS.Infrastructure.Localization;

namespace TMS.RootComponents
{

    public class AppStateHandler
    {
       private readonly IStringLocalizer<ViewResources> _viewLocalizer;

        public string AppTitle { get; set; }=string.Empty;
        public bool DrawerOpen { get; set; } = true;

        public event Action? OnDrawerToggle;
        public event Action? OnAppTitleChanged;

        public AppStateHandler()
        {
            //AppTitle = _viewLocalizer["ApplicationTitle"];
        }



        public void OpenDrawer()
        {     
            DrawerOpen = !DrawerOpen;
            OnDrawerToggle?.Invoke();
        }
        public void SetAppTitle(string title)
        {
            AppTitle = title;
            OnAppTitleChanged?.Invoke();
        }
        public void SetAppTitleDefault(string title)
        {
            AppTitle = _viewLocalizer["ApplicationTitle"];
            OnAppTitleChanged?.Invoke();
        }
    }
}
