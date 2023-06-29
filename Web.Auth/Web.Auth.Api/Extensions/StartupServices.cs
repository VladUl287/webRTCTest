using Web.Auth.Core.Entities;
using Web.Auth.Core.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Web.Auth.Api.Extensions;

internal static class StartupServices
{
    public static void AddDefaultMvc(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddRazorPages();
    }

    public static void AddDatabase<TContext, TAssemblyMarker>(this IServiceCollection services, IConfiguration configuration)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                options => options.MigrationsAssembly(typeof(TAssemblyMarker).Assembly.FullName));

            options.UseOpenIddict();

            options.LogTo(Console.WriteLine, LogLevel.Information);
        });
    }

    public static void AddOpenId<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<TContext>()
            .AddDefaultTokenProviders();

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<TContext>();
            })
            .AddServer(options =>
            {
                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(15));
                // options.SetRefreshTokenLifetime(TimeSpan.FromDays(7));

                // options.SetAccessTokenLifetime(TimeSpan.FromSeconds(70));
                options.SetRefreshTokenLifetime(TimeSpan.FromDays(7));

                options.SetAuthorizationEndpointUris("connect/authorize")
                    .SetTokenEndpointUris("connect/token")
                    .SetIntrospectionEndpointUris("connect/introspect")
                    .SetLogoutEndpointUris("connect/logout")
                    .SetUserinfoEndpointUris("connect/userinfo")
                    ;

                options.RegisterScopes(Scopes.Profile);

                options.AllowAuthorizationCodeFlow()
                    .AllowRefreshTokenFlow();

                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableStatusCodePagesIntegration()
                    .EnableTokenEndpointPassthrough()
                    .EnableLogoutEndpointPassthrough()
                    .EnableUserinfoEndpointPassthrough()
                    ;
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });
    }

    public static void AddDefaultCors(this IServiceCollection services, IConfiguration configuration)
    {
        var cors = configuration
            .GetSection(CorsConfiguration.Position)
            .Get<CorsConfiguration>();

        if (cors is null or { Origins.Length: 0 })
        {
            throw new NullReferenceException("Cors configuration not found or not correct.");
        }

        services.AddCors(setup =>
        {
            setup.AddDefaultPolicy(config =>
            {
                config.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin();
                // .AllowCredentials()
                // .WithOrigins(cors.Origins);
            });
        });
    }
}
