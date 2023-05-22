using Microsoft.EntityFrameworkCore;
using Web.Hubs.Core.Configuration;

namespace Web.Hubs.Api.Extensions;

internal static class StartupServices
{
    public static void AddDatabase<TContext, TAssemblyMarker>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                options => options.MigrationsAssembly(typeof(TAssemblyMarker).Assembly.FullName));
        });
    }

    public static void AddDefaultCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsConfig = configuration
            .GetSection(CorsConfiguration.Position)
            .Get<CorsConfiguration>();

        if (corsConfig is null)
        {
            throw new NullReferenceException("Cors configuration not found.");
        }

        services.AddCors(setup =>
        {
            setup.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(corsConfig.Origins);
            });
        });
    }
}