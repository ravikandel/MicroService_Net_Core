using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;

namespace AuthService.Configurations
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
                        Title = "Auth Service API",
                        Version = description.ApiVersion.ToString(),
                        Description = $"Auth Service API Swagger for version {description.ApiVersion}"
                    });

                options.DocumentFilter<BasePathFilter>("/auth"); // Custom filter to rewrite paths
            }

            // Optional: Hide schema/model section
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                return apiDesc.GroupName == docName;
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