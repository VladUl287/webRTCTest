using Microsoft.AspNetCore;
using System.Security.Claims;
using Web.Auth.Core.Entities;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Authentication;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Web.Auth.Api;

public sealed class AuthorizeController : Controller
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictAuthorizationManager _authorizationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AuthorizeController(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager,
        SignInManager<User> signInManager,
        UserManager<User> userManager)
    {
        _applicationManager = applicationManager;
        _authorizationManager = authorizationManager;
        _scopeManager = scopeManager;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize"), IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {
        var request = GetOpenIdRequest();

        var authResult = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

        if (authResult is null or { Succeeded: false } || request.HasPrompt(Prompts.Login))
        {
            return RedirectToLogin(request);
        }

        var user = await _userManager.GetUserAsync(authResult.Principal)
            ?? throw new InvalidOperationException("The user details not found.");

        ArgumentNullException.ThrowIfNullOrEmpty(request.ClientId);

        var application = await _applicationManager.FindByClientIdAsync(request.ClientId)
            ?? throw new InvalidOperationException("Details client application cannot be found.");

        var subject = await _userManager.GetUserIdAsync(user);

        var clientApp = await _applicationManager.GetIdAsync(application)
            ?? throw new InvalidOperationException("Client application identifier not found.");

        var scopes = request.GetScopes();

        var authorizations = await _authorizationManager
            .FindAsync(subject, clientApp, Statuses.Valid, AuthorizationTypes.Permanent, scopes)
            .ToListAsync();

        var identity = await GetAuthorizeIdentity(user, subject, clientApp, scopes, authorizations);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpPost("~/connect/token"), IgnoreAntiforgeryToken]
    public async Task<IActionResult> Exchange()
    {
        var request = GetOpenIdRequest();

        if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType())
        {
            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        var authResult = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        if (authResult is null or { Principal: null })
        {
            return Forbid();
        }

        var subjectClaim = authResult.Principal.GetClaim(Claims.Subject) ?? string.Empty;

        var user = await _userManager.FindByIdAsync(subjectClaim);

        if (user is null)
        {
            var props = new Dictionary<string, string?>()
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The token is no longer valid."
            };

            return Forbid(new AuthenticationProperties(props), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var canSignIn = await _signInManager.CanSignInAsync(user);

        if (!canSignIn)
        {
            var props = new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
            };

            return Forbid(new AuthenticationProperties(props), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var identity = await GetTokenIdentity(authResult, user);

        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    #region Helpers

    private OpenIddictRequest GetOpenIdRequest()
    {
        return HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("The OpenID Connect request could not be received.");
    }

    private async Task<ClaimsIdentity> GetAuthorizeIdentity(User user, string subject, string clientApp, ImmutableArray<string> scopes, List<object> authorizations)
    {
        var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

        var name = await _userManager.GetUserNameAsync(user);
        var email = await _userManager.GetEmailAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        identity.SetClaim(Claims.Subject, subject)
                .SetClaim(Claims.Name, name)
                .SetClaim(Claims.Email, email)
                .SetClaims(Claims.Role, roles.ToImmutableArray());

        identity.SetScopes(scopes);

        var resources = await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync();

        identity.SetResources(resources);

        var authorization = authorizations.LastOrDefault();

        authorization ??= await _authorizationManager.CreateAsync(identity, subject, clientApp, AuthorizationTypes.Permanent, identity.GetScopes());

        identity.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));
        identity.SetDestinations(GetDestinations);

        return identity;
    }

    private async Task<ClaimsIdentity> GetTokenIdentity(AuthenticateResult authResult, User user)
    {
        var identity = new ClaimsIdentity(authResult.Principal!.Claims, TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

        var subject = await _userManager.GetUserIdAsync(user);
        var name = await _userManager.GetUserNameAsync(user);
        var email = await _userManager.GetEmailAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        identity.SetClaim(Claims.Subject, subject)
                .SetClaim(Claims.Email, name)
                .SetClaim(Claims.Name, email)
                .SetClaims(Claims.Role, roles.ToImmutableArray());

        identity.SetDestinations(GetDestinations);

        return identity;
    }

    private IActionResult RedirectToLogin(OpenIddictRequest request)
    {
        var prompts = request
            .GetPrompts()
            .Remove(Prompts.Login);

        var prompt = string.Join(" ", prompts);

        var parameters = new List<KeyValuePair<string, StringValues>>()
            {
                KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt))
            };

        if (Request.HasFormContentType)
        {
            parameters = Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList();
        }
        else
        {
            parameters = Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();
        }

        var authPropertiesRedirect = new AuthenticationProperties
        {
            RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
        };

        return Challenge(authPropertiesRedirect, IdentityConstants.ApplicationScheme);
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        switch (claim.Type)
        {
            case Claims.Name:
                yield return Destinations.AccessToken;

                if (claim.Subject is not null && claim.Subject.HasScope(Scopes.Profile))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (claim.Subject is not null && claim.Subject.HasScope(Scopes.Email))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (claim.Subject is not null && claim.Subject.HasScope(Scopes.Roles))
                {
                    yield return Destinations.IdentityToken;
                }

                yield break;

            case "AspNet.Identity.SecurityStamp": yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }

    #endregion
}
