using Refit;
using Microsoft.EntityFrameworkCore;
using Web.Hubs.Infrastructure.Services;
using Web.Hubs.Infrastructure.Repositories;
using Web.Hubs.Api.HttpHandlers;
using Web.Hubs.Api.Configuration;
using OpenIddict.Validation.AspNetCore;
using Web.Hubs.Core.Contracts.Repositories;
using Web.Hubs.Core.Contracts.Services;
using Web.Hubs.Core.Services;
using StackExchange.Redis;
using Web.Hubs.Core.Contracts;
using Web.Hubs.Infrastructure;
using Web.Hubs.Core.Proxies;

namespace Web.Hubs.Api.Extensions;

internal static class StartupServices
{
    public static void AddDatabase<TContext, TAssemblyMarker>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(
            options =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    options =>
                    {
                        options.MigrationsAssembly(typeof(TAssemblyMarker).Assembly.FullName);
                    });
                options.LogTo(Console.WriteLine, LogLevel.Information);
            },
            ServiceLifetime.Scoped
        );
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

        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
    }

    public static void AddDefaultCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsConfiguration = configuration
            .GetSection(CorsConfiguration.Position)
            .Get<CorsConfiguration>();

        if (corsConfiguration is null or { Origins.Length: 0 })
        {
            throw new NullReferenceException("Cors configuration not found or not correct.");
        }

        services.AddCors(setup => setup
            .AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins(corsConfiguration.Origins);
                }
            )
        );
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IChatManager, ChatManager>();
        services.AddScoped<IChatPresenter, ChatPresenter>();

        services.AddScoped<IMessageManager, MessageManager>();
        services.AddScoped<IMessagePresenter, MessagePresenter>();

        services.AddScoped<ICallManager, CallManager>();
        services.AddScoped<ICallPresenter, CallPresenter>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton(factory => ConnectionMultiplexer.Connect("localhost:6379,allowAdmin=true"));

        services.AddSingleton<IStorage<long>, Storage>();

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IChatUserService, ChatUserService>();

        services.AddScoped<ICallService, CallService>();

        services.AddScoped<IMessageService, MessageService>();
    }

    public static void AddRefit(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddTransient<AuthHeaderHandler>();

        services.AddRefitClient<IAuthApi>()
            .ConfigureHttpClient(config =>
            {
                config.BaseAddress = new Uri("https://localhost:7250/api");
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();
    }
}