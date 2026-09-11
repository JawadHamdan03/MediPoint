using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MediPoint.Api.Registerations;

public static class AuthRegisterationsExtensionMethods
{
    public static IServiceCollection AddAuthRegisterations(this IServiceCollection service, IConfiguration configuration)
    {
        
        service.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var JwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = JwtSettings["SecretKey"];
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                ValidIssuer = JwtSettings["Issuer"],
                ValidAudience = JwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),

            };
        });

        service.AddAuthorization();

        return service;
    }
}