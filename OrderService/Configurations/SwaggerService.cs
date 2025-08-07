using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace OrderService.Configurations
{
    public class SwaggerService(IApiVersionDescriptionProvider provider) : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo()
                    {
                        Title = "Order Service API",
                        Version = description.ApiVersion.ToString(),
                        Description = $"Order Service API Swagger for version {description.ApiVersion}"
                    });
                options.DocumentFilter<BasePathFilter>("/order"); // Custom filter to rewrite paths
            }

            // Optional: Filter endpoints to match doc version
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                return apiDesc.GroupName == docName;
            });

            // Add JWT Bearer Token Auth Support
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter the JWT token only.\n\nExample: **eyJhbGciOiJIUzI1NiIs...**"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }
    }
    public class BasePathFilter : IDocumentFilter
    {
        private readonly string _basePath;

        public BasePathFilter(string basePath)
        {
            _basePath = basePath;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var paths = swaggerDoc.Paths.ToDictionary(
                path => _basePath + path.Key,
                path => path.Value
            );
            swaggerDoc.Paths = new OpenApiPaths();
            foreach (var path in paths)
                swaggerDoc.Paths.Add(path.Key, path.Value);
        }
    }
}