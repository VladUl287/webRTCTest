using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Web.Auth.Core.Entities;

namespace Web.Auth.Api.Pages.Account;

public sealed class LoginModel : PageModel
{
    private readonly SignInManager<User> signInManager;

    public LoginModel(SignInManager<User> signInManager)
    {
        this.signInManager = signInManager;
    }

    public string ReturnUrl { get; set; } = string.Empty;

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    [BindProperty]
    public required InputModel Input { get; set; }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");

        if (ModelState.IsValid)
        {
            var result = await signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return LocalRedirect(ReturnUrl);
            }

            // if (result.RequiresTwoFactor)
            // {
            //     return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
            // }

            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }

            ModelState.AddModelError(string.Empty, "Ошибка авторизации.");
        }

        return Page();
    }

    public sealed class InputModel
    {
        [Required(ErrorMessage = "email required")]
        [EmailAddress(ErrorMessage = "invalid email")]
        public required string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "password required")]
        [DataType(DataType.Password)]
        public required string Password { get; set; } = string.Empty;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
