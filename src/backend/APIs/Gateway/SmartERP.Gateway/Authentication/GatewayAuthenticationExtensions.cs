using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SmartERP.Gateway.Authentication;

public static class GatewayAuthenticationExtensions
{
    public static WebApplicationBuilder AddGatewayAuthentication(
        this WebApplicationBuilder builder)
    {
        var jwtSection = builder.Configuration.GetRequiredSection(
            GatewayJwtOptions.SectionName);

        var jwtOptions = jwtSection.Get<GatewayJwtOptions>()
            ?? throw new InvalidOperationException(
                "JWT configuration was not found.");

        var signingKeyBytes = GetSigningKey(jwtOptions.SigningKey);

        builder.Services
            .AddOptions<GatewayJwtOptions>()
            .Bind(jwtSection)
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Issuer),
                "Jwt:Issuer is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Audience),
                "Jwt:Audience is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.SigningKey),
                "Jwt:SigningKey is required.")
            .ValidateOnStart();

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.SaveToken = false;

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(signingKeyBytes),

                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,

                        NameClaimType = JwtRegisteredClaimNames.Name,
                        RoleClaimType = "role",

                        ValidAlgorithms =
                        [
                            SecurityAlgorithms.HmacSha256
                        ],

                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
            });

        builder.Services.AddAuthorization();

        return builder;
    }

    private static byte[] GetSigningKey(string signingKey)
    {
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey is required.");
        }

        try
        {
            var signingKeyBytes = Convert.FromBase64String(signingKey);

            if (signingKeyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "Jwt:SigningKey must contain at least 32 bytes.");
            }

            return signingKeyBytes;
        }
        catch (FormatException exception)
        {
            throw new InvalidOperationException(
                "Jwt:SigningKey must be a valid Base64 value.",
                exception);
        }
    }
}
