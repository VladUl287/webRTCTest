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
        var headers = contextAccessor.HttpContext?.Request.Headers.Authorization;

        if (headers is { Count: > 0 })
        {
            var value = headers.Value[0];

            if (!string.IsNullOrEmpty(value))
            {
                request.Headers.Add("Authorization", value);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
