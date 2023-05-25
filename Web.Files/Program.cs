using Web.Files;
using Web.Files.Options;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();

    builder.Services.Configure<ImageOptions>(
        builder.Configuration.GetSection(ImageOptions.Position));

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

var app = builder.Build();
{
    app.UseHttpsRedirection();

    // app.UseResponseCaching();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, Configuration.StaticDirectory)),
        RequestPath = $"/{Configuration.StaticDirectory}",
        OnPrepareResponse = (ctx) =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={Configuration.CacheMaxAgeSeconds}");
        }
    });

    app.MapControllers();

    app.Run();
}
