using Web.Auth.Api;
using Web.Auth.Api.Extensions;
using Web.Auth.Infrastructure;
using Web.Auth.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDefaultMvc();

    builder.Services.AddDatabase<DatabaseContext, IInfrastructureMarker>(builder.Configuration);

    builder.Services.AddOpenId();

    builder.Services.AddDefaultCors(builder.Configuration);

    builder.Services.AddHostedService<ClientsInitializeService>();

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
    app.UseStaticFiles();

    app.UseRouting();

    app.UseCors();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();
    app.MapControllers();

    app.Run();
}