namespace Web.Hubs.Api.HttpHandlers;

public sealed class AuthHeaderHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor contextAccessor;

    public AuthHeaderHandler(IHttpContextAccessor contextAccessor)
    {
        this.contextAccessor = contextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = contextAccessor.HttpContext?.Request.Headers["Authorization"];

        if (token.HasValue)
        {
            var value = token.Value.FirstOrDefault()?.Remove(0, 7) ?? string.Empty;

            request.Headers.Authorization = new("Bearer", value);
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
