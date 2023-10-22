using Web.Hubs.Core;
using FluentValidation;
using Web.Hubs.Api.Host;
using Web.Hubs.Api.Hubs;
using Web.Hubs.Infrastructure;
using Web.Hubs.Api.Extensions;
using Web.Hubs.Infrastructure.Database;
using Web.Hubs.Core.Proxies;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();

    builder.Services.AddDefaultCors(builder.Configuration);

    builder.Services.AddDatabase<DatabaseContext, IInfrastructureMarker>(builder.Configuration);

    builder.Services.AddSignalR();
        //.AddMessagePackProtocol();

    builder.Services.AddOpenIdAuthentication();

    builder.Services.AddRepositories();
    builder.Services.AddServices();

    builder.Services.AddValidatorsFromAssembly(typeof(ICoreMarker).Assembly);

    builder.Services.AddHostedService<RedisFlushService>();

    builder.Services.AddRefit();

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

    app.UseCors();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHub<ChatHub>("/hubs/chat");

    app.MapControllers();
}
app.Run();