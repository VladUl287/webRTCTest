﻿using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.DependencyInjection;

namespace Web.Files.Extensions;

internal static class StartupServices
{
    public static void AddImageProcessor(this IServiceCollection services, IWebHostEnvironment environment)
    {
        if (!Directory.Exists(environment.WebRootPath))
        {
            Directory.CreateDirectory(environment.WebRootPath);
        }

        services.AddImageSharp(options =>
            {
                options.Configuration = Configuration.Default;
                options.BrowserMaxAge = TimeSpan.FromDays(7);
                options.CacheMaxAge = TimeSpan.FromDays(365);
            })
            .Configure<PhysicalFileSystemCacheOptions>(options =>
            {
                options.CacheFolder = "is-cache";
                options.CacheFolderDepth = 8;
            })
            // .Configure<PhysicalFileSystemProviderOptions>(options =>
            // {
            //     options.ProviderRootPath = ImagesBasePath;
            // })
            // .AddProvider<PhysicalFileSystemProvider>()
            ;
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
