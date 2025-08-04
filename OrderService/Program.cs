using System.Globalization;
using OrderService.Extensions;

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

builder.Services.AddControllers();

var app = builder.Build();

// Swagger UI configuration
app.ConfigureSwaggerUI();

// Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();