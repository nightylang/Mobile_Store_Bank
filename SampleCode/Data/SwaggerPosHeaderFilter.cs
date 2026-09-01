using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MobileStoreBank.Data
{
    public class SwaggerPosHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();

            // 1. Force inject the Hardware Terminal identifier header mapping field
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-POS-Terminal-ID",
                In = ParameterLocation.Header,
                Description = "Unique point-of-sale device hardware identifier tracking tag node.",
                Required = true,
                Schema = new OpenApiSchema { Type = "string", Default = new OpenApiString("POS-TERMINAL-77") }
            });

            // 2. Force inject the Asymmetric cleartext channel token security verification key field
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-POS-Security-Token",
                In = ParameterLocation.Header,
                Description = "Cryptographic access validation verification string required on unencrypted channels.",
                Required = true,
                Schema = new OpenApiSchema { Type = "string", Default = new OpenApiString("POS-SECURE-KEY-HASH-V2") }
            });
        }
    }
}
