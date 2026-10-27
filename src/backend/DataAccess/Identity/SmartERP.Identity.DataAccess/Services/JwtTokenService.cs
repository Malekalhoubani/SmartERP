using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartERP.Identity.DataAccess.Options;
using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Services.Interfaces;
using SmartERP.Identity.Services.Models;

namespace SmartERP.Identity.DataAccess.Services;

public sealed class JwtTokenService : ITokenService
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenService(
        IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public AccessTokenResult GenerateAccessToken(
        ApplicationUser user,
        IReadOnlyCollection<string> roles,
        long sessionId)
    {
        var issuedAtUtc = DateTimeOffset.UtcNow;

        var expiresAtUtc = issuedAtUtc.AddMinutes(
            _jwtOptions.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email!),

            new(
                JwtRegisteredClaimNames.Name,
                user.FullName),

            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString("N")),

            new(
                "sid",
                sessionId.ToString(),
                ClaimValueTypes.Integer64),

            new(
                JwtRegisteredClaimNames.Iat,
                issuedAtUtc.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),

            new(
                "email_verified",
                user.EmailConfirmed.ToString().ToLowerInvariant(),
                ClaimValueTypes.Boolean)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim("role", role));
        }

        var signingKeyBytes =
            Convert.FromBase64String(_jwtOptions.SigningKey);

        var securityKey =
            new SymmetricSecurityKey(signingKeyBytes);

        var signingCredentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: issuedAtUtc.UtcDateTime,
            expires: expiresAtUtc.UtcDateTime,
            signingCredentials: signingCredentials);

        var serializedToken =
            new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return new AccessTokenResult(
            serializedToken,
            expiresAtUtc);
    }
}
