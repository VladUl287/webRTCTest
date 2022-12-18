using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Web.Auth.Core.Configuration;
using Web.Auth.Core.Contracts;
using Web.Auth.Infrastructure.Database;
using Web.Auth.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
{
    var tokenSection = builder.Configuration.GetSection(TokenConfiguration.Position);

    builder.Services.Configure<TokenConfiguration>(tokenSection);

    var tokenConfiguration = tokenSection.Get<TokenConfiguration>()!;

    builder.Services.AddControllers();

    builder.Services.AddDbContext<DatabaseContext>(options =>
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            options => options.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName));
    });

    builder.Services.AddCors();

    builder.Services.AddScoped<IUserService, UserService>();

    var byteKey = Encoding.UTF8.GetBytes(tokenConfiguration.AccessKey);
    builder.Services.AddAuthentication()
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = tokenConfiguration.Issuer,
                ValidAudience = tokenConfiguration.Audience,
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
var corsConfiguration = builder.Configuration
    .GetSection(CorsConfiguration.Position)
    .Get<CorsConfiguration>()!;
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
            .WithOrigins(corsConfiguration.Origins);
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