using System.Text;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
{
    var configuration = new
    {
        SecurityKey = "",
        Issuer = "",
        Audience = ""
    };

    builder.Services.AddControllers();

    builder.Services.AddCors();

    var byteKey = Encoding.UTF8.GetBytes(configuration.SecurityKey);
    builder.Services.AddAuthentication()
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = configuration.Issuer,
                ValidAudience = configuration.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(byteKey)
            };
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseCors(config =>
    {
        config.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(new string[] { });
    });

    app.UseCookiePolicy(new CookiePolicyOptions
    {
        MinimumSameSitePolicy = SameSiteMode.None,
        HttpOnly = HttpOnlyPolicy.Always,
        Secure = CookieSecurePolicy.Always
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}