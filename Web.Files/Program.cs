using Web.Files.Options;
using Web.Files.Extensions;
using SixLabors.ImageSharp.Web.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();

    builder.Services.Configure<ImageOptions>(
        builder.Configuration.GetSection(ImageOptions.Position));

    builder.Services.AddImageProcessor(builder.Environment);

    builder.Services.AddOpenIdAuthentication();
}

var app = builder.Build();
{
    app.UseHttpsRedirection();

    // app.UseAuthentication();
    // app.UseAuthorization();

    // app.UseStaticFiles();

    app.UseImageSharp();

    app.MapControllers();
}
app.Run();
