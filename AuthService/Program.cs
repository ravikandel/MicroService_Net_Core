using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using AuthService.Data;
using AuthService.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port from AppSettings -> ServicePort
// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenLocalhost(builder.Configuration.GetValue<int>("ServicePort"));
// });

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sql =>
    {
        sql.MigrationsHistoryTable("__EFMigrationsHistory", "Auth");
    }));

builder.Services.RegisterLogicAndRepository();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerService>();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    $"Auth Service API {description.GroupName}");
            }

            options.RoutePrefix = "";
            options.DefaultModelsExpandDepth(-1);
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        });
// }

app.UseAuthentication();
app.UseAuthorization();

// if (app.Environment.IsProduction())
// {
//     app.UseHttpsRedirection();
// }

app.MapControllers();

app.Run();
