using WebHub.App.Hubs;
using Web.Hub.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Web.Hub.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();

    var cors = new string[] { "http://localhost:5173" };

    builder.Services.AddCors(setup =>
    {
        setup.AddDefaultPolicy(config =>
        {
            config.AllowAnyMethod()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins(cors);
        });
    });

    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            options => options.MigrationsAssembly(typeof(IInfrastructureMarker).Assembly.FullName)));

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