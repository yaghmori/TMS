using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TMS.API.Host.Helpers
{
    public class OrderTagsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc,
                   DocumentFilterContext context)
        {
            swaggerDoc.Tags = swaggerDoc.Tags.OrderBy(x =>
                        x.Name).ToList();
        }
    }

}
