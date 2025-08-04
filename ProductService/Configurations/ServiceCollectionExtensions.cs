using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using ProductService.Configurations;
using ProductService.Common;
using ProductService.Data;
using System.Text;

namespace ProductService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ProductDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"), sql =>
                    sql.MigrationsHistoryTable("__EFMigrationsHistory", "Product")));

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = config["JwtConfig:Issuer"]!.ToLower(),
                        ValidateAudience = true,
                        ValidAudience = config["JwtConfig:Audience"]!.ToLower(),
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtConfig:Key"]!)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddSwaggerWithVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerService>();
            services.AddSwaggerGen();

            return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.RegisterLogicAndRepository(); // your custom extension for DI
            return services;
        }

        public static IServiceCollection ConfigureCustomOptions(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<ApiGatewayOptions>(config.GetSection("ApiGateway"));
            return services;
        }
    }
}
