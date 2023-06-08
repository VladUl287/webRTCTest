using Web.Auth.Core.Entities;
using Web.Auth.Core.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Auth.Infrastructure.Database;
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
        });
    }

    public static void AddOpenId(this IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<DatabaseContext>();
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("connect/authorize")
                    .SetLogoutEndpointUris("connect/logout")
                    .SetTokenEndpointUris("connect/token")
                    // .SetUserinfoEndpointUris("connect/userinfo")
                    ;

                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

                options.AllowAuthorizationCodeFlow()
                    .AllowRefreshTokenFlow();

                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableLogoutEndpointPassthrough()
                    .EnableStatusCodePagesIntegration()
                    .EnableTokenEndpointPassthrough()
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

        if (cors is null)
        {
            throw new NullReferenceException("Cors configuration not found.");
        }

        services.AddCors(setup =>
        {
            setup.AddDefaultPolicy(config =>
            {
                config.AllowAnyMethod()
                    .AllowAnyMethod()
                    .AllowAnyOrigin();
                    // .AllowCredentials()
                    // .WithOrigins(cors.Origins);
            });
        });
    }
}
