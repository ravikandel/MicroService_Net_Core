using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace ProductService.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication ConfigureSwaggerUI(this WebApplication app)
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        $"Product Service API {description.GroupName}");
                }

                options.RoutePrefix = "";
                options.DefaultModelsExpandDepth(-1);
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            });

            return app;
        }
    }
}
