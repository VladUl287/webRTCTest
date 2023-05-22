using Web.Hubs.Api.Hubs;
using Web.Hubs.Infrastructure;
using Web.Hubs.Api.Extensions;
using Web.Hubs.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();

    builder.Services.AddDefaultCors(builder.Configuration);

    builder.Services.AddDatabase<DatabaseContext, IInfrastructureMarker>(builder.Configuration);

    builder.Services.AddSignalR()
        .AddMessagePackProtocol();

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

    app.MapHub<RoomHub>("/room");

    app.MapControllers();

    app.Run();
}