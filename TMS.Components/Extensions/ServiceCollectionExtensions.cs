using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MudBlazor;
using MudBlazor.Services;
using System.Reflection;
using TMS.Core.Application.Authorization;
using TMS.Core.Application.Extensions;
using TMS.Shared.Constants;
using TMS.Shared.MappingProfile;

namespace TMS.RootComponents.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRootComponentService(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddBlazoredLocalStorage();
            services.AddBlazoredSessionStorage();

            services.AddSingleton<AppStateHandler>();
            services.AddScoped<AuthStateProvider>();
            services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            services.AddScoped<IClaimsTransformation, ApplicationClaimsTransformation>();

            services.AddScoped<IAuthorizationHandler, PermissionRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, TenantMemberRequirementHandler>();

            services.AddAuthorizationCore(options =>
            {
                options.InvokeHandlersAfterFailure = true;

                //TenantMemberPolicy
                options.AddPolicy(ApplicationPolicy.TenantMember, policy =>
                policy.RequireAuthenticatedUser().Requirements.Add(new TenantMemberRequirement()));

                //PemissionPolicy
                var permissionList = typeof(ApplicationPermissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy));
                foreach (var permission in permissionList)
                {
                    var propertyValue = permission.GetValue(null);
                    options.InvokeHandlersAfterFailure = true;

                    if (propertyValue is not null)
                    {
                        options.AddPolicy(propertyValue.ToString(), policy => policy.RequireAuthenticatedUser()
                        .Requirements.Add(new PermissionRequirement(propertyValue.ToString())));
                    }
                }

            });
            services.AddAuthentication();
            services.AddApplicationService();
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                config.SnackbarConfiguration.BackgroundBlurred = true;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.ClearAfterNavigation = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 7000;
                config.SnackbarConfiguration.HideTransitionDuration = 300;
                config.SnackbarConfiguration.ShowTransitionDuration = 300;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });

            return services;


        }
    }
}
