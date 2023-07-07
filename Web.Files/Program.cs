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

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // app.UseAuthentication();
    // app.UseAuthorization();

    app.UseImageSharp();

    app.UseStaticFiles();

    app.MapControllers();
}
app.Run();
