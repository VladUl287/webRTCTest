using Microsoft.AspNetCore.Identity;
using Web.Auth.Core.Entities;

namespace Web.Auth.Api;

public class UsersInitService : IHostedService
{
    private readonly IServiceProvider serviceProvider;

    public UsersInitService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateAsyncScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var emails = new[]
        {
            "first.01@mail.ru",
            "second.01@mail.ru"
        };

        foreach (var email in emails)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                user = new User
                {
                    Email = email,
                    UserName = email
                };

                await userManager.CreateAsync(user, "testPassword1@");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
