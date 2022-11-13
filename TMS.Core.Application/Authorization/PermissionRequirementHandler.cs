using Microsoft.AspNetCore.Authorization;
using TMS.Shared.Constants;

namespace TMS.Core.Application.Authorization
{

    public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
    {

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                await Task.CompletedTask;
                return;
            }
          
            if (context.User.Claims.Any(c => c.Type == ApplicationClaimTypes.Permission && c.Value == requirement.Permission))
                context?.Succeed(requirement);
            await Task.CompletedTask;
            return;
        }
    }

}
