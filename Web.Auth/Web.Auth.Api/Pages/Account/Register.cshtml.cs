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

    // public IList<AuthenticationScheme> ExternalLogins { get; set; }

    public Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        return Task.CompletedTask;

        // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (ModelState.IsValid)
        {
            var user = new Core.Entities.User();

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                var userId = await _userManager.GetUserIdAsync(user);

                // if (_userManager.Options.SignIn.RequireConfirmedAccount)
                // {
                //     return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                // }

                await _signInManager.SignInAsync(user, isPersistent: false);

                return LocalRedirect(ReturnUrl);
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

    public sealed class InputModel
    {
        [Required(ErrorMessage = "email required")]
        [EmailAddress(ErrorMessage = "invalid email")]
        [Display(Prompt = "example@example.org")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "password required")]
        [StringLength(100, ErrorMessage = "minimum 6 characters", MinimumLength = 6)]
        [Display(Prompt = "password")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "confirm required")]
        [DataType(DataType.Password)]
        [Display(Prompt = "confirm")]
        [Compare("Password", ErrorMessage = "password mismatch")]
        public required string ConfirmPassword { get; set; }
    }
}
