using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StoreMobile.Data
{
    public class SwaggerAttendanceHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Apply this header filter descriptor strictly to our attendance scan endpoint route
            if (context.ApiDescription.RelativePath != null && 
                context.ApiDescription.RelativePath.Contains("attendance/process-scan"))
            {
                operation.Parameters ??= new List<OpenApiParameter>();

                // Clear out unnecessary POS terminal header configurations for the attendance section
                operation.Parameters.Clear();

                operation.Summary = "Asynchronous attendance logging route for local Python OpenCV badge reader devices";
                operation.Description = "Processes raw scanned alphanumeric tokens on an unencrypted HTTP cleartext pipe, calculating check-in/out states dynamically.";
            }
        }
    }
}
