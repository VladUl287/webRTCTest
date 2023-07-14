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
        var headers = contextAccessor.HttpContext?.Request.Headers["Authorization"];

        if (headers.HasValue)
        {
            var value = headers.Value.FirstOrDefault() ?? string.Empty;

            request.Headers.Add("Authorization", value);

            if (!string.IsNullOrEmpty(value))
            {
                // var token = value.Replace("Bearer", string.Empty);

                // request.Headers.Authorization = new("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
