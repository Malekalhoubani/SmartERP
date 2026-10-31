using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SmartERP.Inventory.API.Authentication;

public static class InventoryAuthenticationExtensions
{
    public static WebApplicationBuilder AddInventoryAuthentication(this WebApplicationBuilder builder)
    {
        var issuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is required.");
        var audience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is required.");
        var signingKey = builder.Configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey is required.");
        byte[] keyBytes;
        try { keyBytes = Convert.FromBase64String(signingKey); }
        catch (FormatException exception) { throw new InvalidOperationException("Jwt:SigningKey must be a valid Base64 value.",exception); }
        if (keyBytes.Length < 32) throw new InvalidOperationException("Jwt:SigningKey must contain at least 32 bytes.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                NameClaimType = JwtRegisteredClaimNames.Name,
                RoleClaimType = "role",
                ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });
        builder.Services.AddAuthorization();
        return builder;
    }
}
