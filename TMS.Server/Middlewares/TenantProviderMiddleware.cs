using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.IO;
using TMS.DataAccess.Context;
using TMS.Infrastructure.Infrastructure.UnitOfWork;
using TMS.Shared.Constants;

namespace TMS.API.Host.Middlewares
{
    public class TenantProviderMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantProviderMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context, IUnitOfWork<ServerDbContext> unitOfWork)
        {

            //var path = context.Request.Path;
            //if (path.StartsWithSegments("/" + EndPoints.TenantEndPoint))

            var tenantId = context.Request.Headers[ApplicationConstants.TenantId];

            if (!string.IsNullOrWhiteSpace(tenantId))
            {
                var tenant =await unitOfWork.Tenants.FindAsync(Guid.Parse(tenantId));
                if(tenant is not null)
                {
                    TenantDbContext.ConnectionString =tenant.ConnectionString;
                }
            }
            await _next.Invoke(context);
        }

    }
}
