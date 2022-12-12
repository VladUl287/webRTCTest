using WebHub.App.Hubs;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddSignalR()
                    .AddMessagePackProtocol();

    builder.Services.AddCors();
}

var app = builder.Build();
{
    app.UseCors(config =>
    {
        config.AllowAnyHeader();
        config.AllowAnyMethod();
        config.AllowCredentials();
        config.WithOrigins("http://localhost:5173");
    });

    app.MapHub<RoomHub>("/room");
    
    app.Run();
}