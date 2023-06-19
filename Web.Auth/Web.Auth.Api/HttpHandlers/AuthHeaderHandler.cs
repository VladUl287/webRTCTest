using System.Net.Http.Headers;

namespace Web.Auth.Api.HttpHandlers;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor contextAccessor;

    public AuthHeaderHandler(IHttpContextAccessor contextAccessor)
    {
        this.contextAccessor = contextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = contextAccessor.HttpContext.Request.Headers["Authorization"];

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.FirstOrDefault().Remove(0, 7));

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
