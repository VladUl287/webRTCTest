using Microsoft.EntityFrameworkCore;
using Web.Hubs.Core.Configuration;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Services;
using Web.Hubs.Infrastructure.Database;

namespace Web.Hubs.Api.Extensions;

internal static class StartupServices
{
    public static void AddDatabase<TContext, TAssemblyMarker>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext, IUnitOfWork
    {
        const int RetryCount = 2;

        services.AddDbContext<TContext>(
            options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    options =>
                    {
                        options.MigrationsAssembly(typeof(TAssemblyMarker).Assembly.FullName);
                        options.EnableRetryOnFailure(RetryCount);
                    });
            },
            ServiceLifetime.Scoped
        );

        services.AddScoped<IUnitOfWork, TContext>();
    }

    public static void AddOpenIdAuthentication(this IServiceCollection services)
    {
        // services.AddOpenIddict()
        // 		.AddValidation(options =>
        // 		{
        // 			options.SetIssuer("");
        // 			options.AddAudiences("");

        // 			options.UseIntrospection()
        // 				.SetClientId("")
        // 				.SetClientSecret("");

        // 			options.UseSystemNetHttp();
        // 			options.UseAspNetCore();
        // 		});

        // services.AddAuthentication(options =>
        // {
        //     options.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        // });
    }

    public static void AddDefaultCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsConfig = configuration
            .GetSection(CorsConfiguration.Position)
            .Get<CorsConfiguration>();

        if (corsConfig is null or { Origins.Length: 0 })
        {
            throw new NullReferenceException("Cors configuration not found or not correct.");
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

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IChatPresenter>();
        services.AddScoped<IMessagePresenter>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IChatService>();
        services.AddScoped<IMessageService>();
        services.AddScoped<IStoreService<long, string>>();
    }
}