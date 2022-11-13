using Microsoft.AspNetCore.Authorization;
using TMS.DataAccess.Context;
using TMS.Infrastructure.Infrastructure.UnitOfWork;
using TMS.Shared.Constants;

namespace TMS.Core.Services.Authorization
{

    public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUnitOfWork<ServerDbContext> _unitOfWork;
        public PermissionRequirementHandler(IUnitOfWork<ServerDbContext> unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null)
            {
                await Task.CompletedTask;
                return;
            }
            var userId = context?.User?.FindFirst(c => c.Type == ApplicationClaimTypes.UserId)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                await Task.CompletedTask;
                return;
            }

            var result = await _unitOfWork.UserClaims.GetAllAsync(predicate: x => x.UserId == Guid.Parse(userId));
            //    selector: s => SelectExpressions.UserClaims.UserClaimResponse);
            if (result.ToList().Any(c => c.ClaimType == ApplicationClaimTypes.Permission && c.ClaimValue == requirement.Permission))
                context?.Succeed(requirement);

            await Task.CompletedTask;
            return;
        }
    }

}
