using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using AuthService.Configurations;
using AuthService.Data;

namespace AuthService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"), sql =>
                    sql.MigrationsHistoryTable("__EFMigrationsHistory", "Auth")));

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
            services.RegisterLogicAndRepository(); // your custom extension for DI
            return services;
        }

    }
}
