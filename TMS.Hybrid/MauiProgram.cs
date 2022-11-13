using Microsoft.AspNetCore.Components.Authorization;
using TreeFMS.Shared;
using TreeFMS.Shared.Services;

namespace TreeFMS.Hybrid
{
    public static class MauiProgram
    {



        public static MauiApp CreateMauiApp()
        {

            Shared.ApplicationSettings.BaseUrl = DeviceInfo.Platform == DevicePlatform.Android ? "10.0.2.2" : "localhost";
            Shared.ApplicationSettings.BaseAddress = $"http://{Shared.ApplicationSettings.BaseUrl}:7001/";
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddSharedService();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

            return builder.Build();
        }
    }
}