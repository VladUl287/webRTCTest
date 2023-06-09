using Web.Auth.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Web.Auth.Api.Pages.Account;

public sealed class RegisterModel : PageModel
{
    private readonly IUserStore<User> _userStore;
    private readonly UserManager<User> _userManager;
    private readonly IUserEmailStore<User> _emailStore;
    private readonly SignInManager<User> _signInManager;

    public RegisterModel(IUserStore<User> userStore, UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userStore = userStore;
        _userManager = userManager;
        _signInManager = signInManager;

        _emailStore = GetEmailStore();
    }

    public string ReturnUrl { get; set; } = string.Empty;

    [BindProperty]
    public required InputModel Input { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    // public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        return Task.CompletedTask;

        // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (ModelState.IsValid)
        {
            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                var userId = await _userManager.GetUserIdAsync(user);

                // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                // code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                // var callbackUrl = Url.Page("/Account/ConfirmEmail", null, protocol: Request.Scheme,
                //     values: new
                //     {
                //         // area = "Identity",
                //         code = code,
                //         userId = userId,
                //         returnUrl = returnUrl
                //     });
                //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email", $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return Page();
    }

    private IUserEmailStore<User> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }

        return (IUserEmailStore<User>)_userStore;
    }

    private static User CreateUser()
    {
        try
        {
            return Activator.CreateInstance<User>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'.");
        }
    }

    public sealed class InputModel
    {
        [Required(ErrorMessage = "email обязателен")]
        [EmailAddress(ErrorMessage = "некорректный email")]
        [Display(Prompt = "example@example.org")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "пароль обязателен")]
        [StringLength(100, ErrorMessage = "минимум 6 символов", MinimumLength = 6)]
        [Display(Prompt = "пароль")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "подтверждение обязательно")]
        [DataType(DataType.Password)]
        [Display(Prompt = "подтверждение")]
        [Compare("Password", ErrorMessage = "пароли не совпадают")]
        public required string ConfirmPassword { get; set; }
    }
}
