using OpenIddict.Abstractions;
using Web.Auth.Infrastructure.Database;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Web.Auth.Api;

internal sealed class ClientsInitializeService : IHostedService
{
    private readonly IServiceProvider serviceProvider;

    public ClientsInitializeService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        //await context.Database.MigrateAsync();

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        const string clinetName = "vue-client";

        var client = await manager.FindByClientIdAsync(clinetName);

        if (client is null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clinetName,
                Type = ClientTypes.Public,
                // ClientSecret = Guid.NewGuid(),
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "Vue client application",
                PostLogoutRedirectUris =
                {
                    new Uri("http://localhost:5173/callback")
                },
                RedirectUris =
                {
                    new Uri("http://localhost:5173/callback")
                },
                Permissions =
                {
                    // Permissions.Scopes.Roles,
                    Permissions.Scopes.Email,
                    // Permissions.Scopes.Profile,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.Logout,
                    // Permissions.ResponseTypes.Code,
                    // Permissions.GrantTypes.RefreshToken,
                    Permissions.Endpoints.Authorization,
                    // Permissions.GrantTypes.AuthorizationCode
                },
                Requirements =
                {
                    // Requirements.Features.ProofKeyForCodeExchange
                }
            });
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
