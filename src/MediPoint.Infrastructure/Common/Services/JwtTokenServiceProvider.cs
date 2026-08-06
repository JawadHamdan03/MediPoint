using Azure.Core;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using MediPoint.Domain.Entities.RefreshToken;
using MediPoint.Domain.Entities.User;
using MediPoint.Domain.Entities.User.Shared;
using MediPoint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MediPoint.Infrastructure.Common.Services;

public class JwtTokenServiceProvider(AppDbContext dbContext,IConfiguration configuration) : IJwtTokenServiceProvider
{
    public async Task<JwtTokenResponse> GenerateJwtToken(BaseUser request)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");

        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var key = jwtSettings["SecretKey"];
        var expiry = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["TokenExpirationInMinutes"]!));


        BaseUser user;
        string role;

        if(request is Admin)
        {
            user = await dbContext.Admins.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(request.Email));
            role = "Admin";
        }
        else if(request is Patient)
        {
            user = await dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(request.Email));
            role = "Patient";
        }
        else
        {
            user = await dbContext.Doctors.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(request.Email));
            role = "Doctor";
        }
            

        //
        var claims = new List<Claim>()
        {
           new Claim(JwtRegisteredClaimNames.Sub,user!.Id.ToString()),
           new Claim(ClaimTypes.Role,role),
        };

        var descriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiry,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
                SecurityAlgorithms.HmacSha256Signature
                )
        };


        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(descriptor);

        var refToken = await GenerateRefreshToken(request);
        return new JwtTokenResponse
        {
            AccessToken = tokenHandler.WriteToken(securityToken),
            RefreshToken = refToken,
            ExpiresAt = expiry
        };

    }

    public async Task<string> GenerateRefreshToken(BaseUser request)
    {
        BaseUser user;
        string role="";

        if (request is Admin)
        {
            user = await dbContext.Admins.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(request.Email));
            role = "Admin";
        }
        else if (request is Patient)
        {
            user = await dbContext.Patients.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(request.Email));
            role = "Patient";
        }
        else
        {
            user = await dbContext.Doctors.AsNoTracking().FirstOrDefaultAsync(u => u.Email.Equals(request.Email));
            role = "Doctor";
        }


        if (user is null)
        {
            throw new Exception("User Was Not Found");
        }

        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        
        if(user is Admin)
        {
            var userRefreshToken = await dbContext.AdminRefreshTokens.Where(r => r.AdminId.Equals(user.Id)).ExecuteDeleteAsync();
            AdminRefreshToken newRefToken = new AdminRefreshToken { AdminId = user.Id, Token = rawToken, ExpiresAt = DateTime.UtcNow.AddDays(7) };
            await dbContext.AdminRefreshTokens.AddAsync(newRefToken);
            await dbContext.SaveChangesAsync(new CancellationToken());
        }

        if(user is Doctor)
        {
            var userRefreshToken = await dbContext.DoctorRefreshTokens.Where(r => r.DoctorId.Equals(user.Id)).ExecuteDeleteAsync();
            DoctorRefreshToken newRefToken = new DoctorRefreshToken { DoctorId = user.Id, Token = rawToken, ExpiresAt = DateTime.UtcNow.AddDays(7) };
            await dbContext.DoctorRefreshTokens.AddAsync(newRefToken);
            await dbContext.SaveChangesAsync(new CancellationToken());
        }
        if(user is Patient)
        {
            var userRefreshToken = await dbContext.PatientRefreshTokens.Where(r => r.PatientId.Equals(user.Id)).ExecuteDeleteAsync();
            PatientRefreshToken newRefToken = new PatientRefreshToken { PatientId = user.Id, Token = rawToken, ExpiresAt = DateTime.UtcNow.AddDays(7) };
            await dbContext.PatientRefreshTokens.AddAsync(newRefToken);
            await dbContext.SaveChangesAsync(new CancellationToken());
        }
        return rawToken;
    }
}
