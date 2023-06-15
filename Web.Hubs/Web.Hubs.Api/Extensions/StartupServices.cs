using Web.Hubs.Core.Services;
using Web.Hubs.Core.Repositories;
using Web.Hubs.Core.Configuration;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Infrastructure.Services;
using Web.Hubs.Infrastructure.Repositories;

namespace Web.Hubs.Api.Extensions;

internal static class StartupServices
{
    public static void AddDatabase<TContext, TAssemblyMarker>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext, IUnitOfWork
    {
        const int EnableRetryOnFailure = 2;

        services.AddDbContext<TContext>(
            options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    options =>
                    {
                        options.MigrationsAssembly(typeof(TAssemblyMarker).Assembly.FullName);
                        options.EnableRetryOnFailure(EnableRetryOnFailure);
                    });
                // options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.LogTo(Console.WriteLine);
            },
            ServiceLifetime.Scoped
        );

        services.AddScoped<IUnitOfWork, TContext>();
    }

    public static void AddOpenIdAuthentication(this IServiceCollection services)
    {
        services.AddOpenIddict()
                .AddValidation(options =>
                {
                    options.SetIssuer("https://localhost:7250/");

                    options.AddAudiences("hubs-api");

                    options.UseIntrospection()
                           .SetClientId("hubs-api")
                           .SetClientSecret("846B62D0-DEF9-4215-A99D-86E6B8DAB342");

                    options.UseSystemNetHttp();
                    options.UseAspNetCore();
                });

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });
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

        services.AddCors(setup => setup
            .AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins(corsConfig.Origins);
                }
            )
        );
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IChatPresenter, ChatPresenter>();
        services.AddScoped<IMessagePresenter, MessagePresenter>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        // services.AddSingleton<ConnectionMultiplexer>(factory => ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true"));

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IUserChatService, UserChatService>();
        // services.AddSingleton<IConnectionService, ConnectionService>();
        services.AddSingleton<IConnectionService, ConnectionServiceDict>();
    }
}