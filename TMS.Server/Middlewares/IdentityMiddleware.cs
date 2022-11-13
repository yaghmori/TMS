using System.Security.Claims;
using System;
using TMS.Infrastructure.Infrastructure.UnitOfWork;
using TMS.DataAccess.Context;
using TMS.Shared.Constants;
using TMS.Shared.Responses;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Infrastructure.Query;
using TMS.Core.Shared.Helpers;

namespace TMS.Web.Server.Middlewares
{
    public class IdentityMiddleware
    {
        private readonly RequestDelegate _next;
        public IdentityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork<ServerDbContext> unitOfWork)
        {
            if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
            {
                // user is not authenticated
                await _next(context);
                return;
            }
            var userId = context.User.FindFirst(x => x.Type.Equals(ApplicationClaimTypes.UserId))?.Value!;
            if (userId == null)
            {
                // not found
                await _next(context);
                return;
            }

            //RoleClaims
            var roles = await unitOfWork.UserRoles.GetAllAsync(predicate: x => x.UserId.ToString().Equals(userId),
                selector: SelectExpressions.UserRoles.RoleResponse);
            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ApplicationClaimTypes.Role, role.Name));
                var items = await unitOfWork.RoleClaims.GetAllAsync(predicate: x => x.RoleId.ToString().Equals(role.Id) ,
                    selector:s=>new Claim(s.ClaimType,s.ClaimValue));
                    roleClaims.AddRange(items);
            }

            //userClaims
            var userClaims = new List<Claim>();

            var permissions = await unitOfWork.UserClaims.GetAllAsync(predicate: x => x.UserId.ToString().Equals(userId),
               selector: s => new Claim(s.ClaimType, s.ClaimValue));
                userClaims.AddRange(permissions);

            //Tenants
            var tenantClaims = new List<Claim>();
            var tenants = await unitOfWork.UserTenants.GetAllAsync(predicate: x => x.UserId.ToString().Equals(userId),
                 selector: s => new Claim(ApplicationClaimTypes.Tenant, s.TenantId.ToString()));
                userClaims.AddRange(tenants);
           

            var claims = new List<Claim>()
                .Union(roleClaims,new ClaimComparer())
                .Union(userClaims,new ClaimComparer())
                .Union(tenantClaims, new ClaimComparer());


            ClaimsIdentity identity = new ClaimsIdentity(claims);
            context.User.AddIdentity(identity);

            await _next(context);
        }

    }
}
