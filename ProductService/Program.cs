using System.Globalization;
using ProductService.Configurations;
using ProductService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port from AppSettings -> ServicePort
// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenLocalhost(builder.Configuration.GetValue<int>("ServicePort"));
// });

// Force invariant culture to fix globalization-invariant mode issue
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// Register all application services
builder.Services
    .AddDatabase(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerWithVersioning()
    .AddAppServices(builder.Configuration)
    .ConfigureCustomOptions(builder.Configuration);



var app = builder.Build();

// Run migrations automatically on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.Migrate();
}

// Swagger UI configuration
app.ConfigureSwaggerUI();

// Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();