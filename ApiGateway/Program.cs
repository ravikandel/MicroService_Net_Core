var builder = WebApplication.CreateBuilder(args);

// Optional: set Kestrel port (you've already done this)
// Configure Kestrel to listen on port from AppSettings -> ServicePort
// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenLocalhost(builder.Configuration.GetValue<int>("ServicePort"));
// });

// Add YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add Swagger UI (no AddSwaggerGen needed unless Gateway has its own APIs)
//builder.Services.AddSwaggerGen(); // Optional

var app = builder.Build();

app.MapGet("/", async context =>
{
    var request = context.Request;
    var baseUrl = $"{request.Scheme}://{request.Host}";

    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync($@"
        <!DOCTYPE html>
        <html>
        <head>
            <title>API Gateway</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background: #f2f2f2;
                    margin: 0;
                    padding: 0;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    height: 100vh;
                }}
                .container {{
                    text-align: center;
                    background: white;
                    padding: 40px;
                    border-radius: 10px;
                    box-shadow: 0 0 10px rgba(0,0,0,0.1);
                }}
                h1 {{
                    color: #007bff;
                }}
                p {{
                    color: #555;
                }}
            </style>
        </head>
        <body>
            <div class='container'>
                <h1>API Gateway is Running.</h1>
                <p>ApiGateway: <a href='{baseUrl}/'>{baseUrl}/</a></p>
                <p>AuthService: <a href='{baseUrl}/auth/'>{baseUrl}/auth/</a></p>
                <p>ProductService: <a href='{baseUrl}/product/'>{baseUrl}/product/</a></p>
                <p>OrderService: <a href='{baseUrl}/order/'>{baseUrl}/order/</a></p>
            </div>
        </body>
        </html>
    ");
});


// Reverse proxy for /product, /order, /auth routing
app.MapReverseProxy();

app.Run();
