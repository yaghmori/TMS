using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TMS.Shared.Constants;

namespace TMS.Web.Server.Swagger
{
    public class HttpHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = ApplicationConstants.TenantId,
                In = ParameterLocation.Header,
                Required = true,
            });
        }
    }

}
