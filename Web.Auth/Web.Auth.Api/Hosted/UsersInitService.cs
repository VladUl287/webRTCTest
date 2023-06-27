using Microsoft.AspNetCore.Identity;
using Web.Auth.Core.Entities;

namespace Web.Auth.Api;

public class UsersInitService : IHostedService
{
    private readonly UserManager<User> _userManager;


    public UsersInitService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var emails = new[]
        {
            "first.01@mail.ru",
            "second.01@mail.ru"
        };

        foreach (var email in emails)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                user = new User
                {
                    Email = email,
                    UserName = email
                };

                await _userManager.CreateAsync(user, "testPassword1@");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
