var builder = WebApplication.CreateBuilder(args);

// Optional: set Kestrel port (you've already done this)
// Configure Kestrel to listen on port from AppSettings -> ServicePort
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(builder.Configuration.GetValue<int>("ServicePort"));
});

// Add YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add Swagger UI (no AddSwaggerGen needed unless Gateway has its own APIs)
//builder.Services.AddSwaggerGen(); // Optional

var app = builder.Build();

// Reverse proxy for /product, /order, /auth routing
app.MapReverseProxy();

app.Run();
