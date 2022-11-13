using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using TMS.Core.Application.DataServices;
using TMS.Shared.Constants;
using TMS.Shared.Handler;
using TMS.Shared.MappingProfile;

namespace TMS.Core.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddHttpClient(ApplicationConstants.ServerHttpClientName, httpClient =>
            {
                httpClient.BaseAddress = new Uri(ApplicationConstants.ServerBaseAddress); ;
            }).AddHttpMessageHandler<AuthorizationMessageHandler>();
            services.AddTransient<AuthorizationMessageHandler>();

            #region DataService
            services.AddScoped<IAuthDataService, AuthDataService>();
            services.AddScoped<ICultureDataService, CultureDataService>();
            services.AddScoped<IUserDataService, UserDataService>();
            services.AddScoped<IRoleDataService, RoleDataService>();
            services.AddScoped<ITenantDataService, TenantDataService>();
            services.AddScoped<ISiloItemDataService, SiloItemDataService>();
            services.AddScoped<IAppSettingDataService, AppSettingDataService>();
            services.AddScoped<IUserSettingDataService, UserSettingDataService>();
            #endregion

            return services;
        }
    }
}
