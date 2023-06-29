using OpenIddict.Abstractions;
using Web.Auth.Infrastructure.Database;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Web.Auth.Api;

internal sealed class ClientsInitService : IHostedService
{
    private readonly IServiceProvider serviceProvider;

    public ClientsInitService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        // await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        //await context.Database.MigrateAsync();

        var appManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        await AddHubApiIfNotExists(appManager);

        await AddVueClientIfNotExists(appManager);

        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

        await RegisterScopesAsync(scopeManager);
    }

    private async Task AddHubApiIfNotExists(IOpenIddictApplicationManager manager)
    {
        const string clinetName = "hubs-api";

        var client = await manager.FindByClientIdAsync(clinetName);

        if (client is null)
        {
            await manager.CreateAsync(new()
            {
                ClientId = clinetName,
                ClientSecret = "846B62D0-DEF9-4215-A99D-86E6B8DAB342",
                Permissions =
                {
                    Permissions.Endpoints.Introspection
                }
            });
        }

    }

    private static async Task AddVueClientIfNotExists(IOpenIddictApplicationManager manager)
    {
        const string clinetName = "vue-client";

        var client = await manager.FindByClientIdAsync(clinetName);

        if (client is null)
        {
            await manager.CreateAsync(new()
            {
                ClientId = clinetName,
                Type = ClientTypes.Public,
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "Vue client application",
                PostLogoutRedirectUris =
                {
                    new Uri("http://127.0.0.1:5173/logout")
                },
                RedirectUris =
                {
                    new Uri("http://127.0.0.1:5173/callback")
                },
                Permissions =
                {
                    Permissions.Scopes.Profile,

                    Permissions.ResponseTypes.Code,

                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Logout,
                    Permissions.Endpoints.Authorization,

                    Permissions.GrantTypes.RefreshToken,
                    Permissions.GrantTypes.AuthorizationCode,

                    Permissions.Prefixes.Scope + "api1"
                },
                Requirements =
                {
                    Requirements.Features.ProofKeyForCodeExchange
                }
            });
        }
    }

    static async Task RegisterScopesAsync(IOpenIddictScopeManager manager)
    {
        if (await manager.FindByNameAsync("api1") is null)
        {
            await manager.CreateAsync(new()
            {
                Name = "api1",
                Resources =
                {
                    "hubs-api"
                }
            });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
