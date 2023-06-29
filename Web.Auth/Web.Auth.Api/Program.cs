using Refit;
using Web.Auth.Api;
using Web.Auth.Api.Extensions;
using Web.Auth.Api.HttpHandlers;
using Web.Auth.Core.Repositories;
using Web.Auth.Infrastructure;
using Web.Auth.Infrastructure.Database;
using Web.Auth.Infrastructure.Repositories;
using Web.Auth.Infrastructure.Proxies;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDefaultMvc();

    builder.Services.AddDatabase<DatabaseContext, IInfrastructureMarker>(builder.Configuration);

    builder.Services.AddOpenId<DatabaseContext>();

    builder.Services.AddDefaultCors(builder.Configuration);

    builder.Services.AddHostedService<ClientsInitService>();
    builder.Services.AddHostedService<UsersInitService>();

    builder.Services.AddScoped<IUsersRepository, UsersRepository>();

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddTransient<AuthHeaderHandler>();

    builder.Services.AddRefitClient<IHubApi>()
        .ConfigureHttpClient(config =>
        {
            config.BaseAddress = new Uri("https://localhost:7010/api");
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

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
}
app.Run();