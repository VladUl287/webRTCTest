var builder = WebApplication.CreateBuilder(args);
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

var app = builder.Build();
{
    app.UseHttpsRedirection();

    // app.UseResponseCaching();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseStaticFiles();

    app.Run();
}
