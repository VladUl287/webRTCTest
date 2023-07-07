using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.DependencyInjection;

namespace Web.Files.Extensions;

internal static class StartupServices
{
    public static void AddImageProcessor(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddImageSharp(options =>
            {
                options.Configuration = Configuration.Default;
                options.BrowserMaxAge = TimeSpan.FromDays(7);
                options.CacheMaxAge = TimeSpan.FromDays(28);
            })
            .Configure<PhysicalFileSystemCacheOptions>(options =>
            {
                options.CacheFolder = "is-cache";
                options.CacheFolderDepth = 8;
            });
    }

    public static void AddOpenIdAuthentication(this IServiceCollection services)
    {
        // services.AddOpenIddict()
        //         .AddValidation(options =>
        //         {
        //             options.SetIssuer("");
        //             options.AddAudiences("");

        //             options.UseIntrospection()
        //                 .SetClientId("")
        //                 .SetClientSecret("");

        //             options.UseSystemNetHttp();
        //             options.UseAspNetCore();
        //         });

        // services.AddAuthentication(options =>
        // {
        //     options.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        // });
    }
}
