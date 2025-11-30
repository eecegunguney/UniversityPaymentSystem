using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
public class GatewayUrlDocumentFilter : IDocumentFilter
{
    private readonly string _gatewayUrl;

    public GatewayUrlDocumentFilter(IConfiguration configuration)
    {
 
        _gatewayUrl = configuration["ApiSettings:InvokeUrl"];
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {

        if (!string.IsNullOrEmpty(_gatewayUrl))
        {
           
            swaggerDoc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = _gatewayUrl,
                    Description = "Azure API Gateway Invoke URL"
                }
            };
        }
    }
}