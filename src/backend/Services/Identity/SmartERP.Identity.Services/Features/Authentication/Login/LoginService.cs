using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Services.Features.Authentication.Login.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartERP.Identity.Services.Interfaces;
using SmartERP.Identity.Services.Features.Authentication.Login.Models;
using SmartERP.Identity.Services.Features.Authentication.Sessions;
namespace SmartERP.Identity.Services.Features.Authentication.Login
{
     public sealed class LoginService : ILoginService
    {
        private readonly IIdentityRepository _identityRepository;
        private readonly ITokenService _tokenService;
        private readonly IAuthenticationSessionService _sessionService;

        public LoginService(
            IIdentityRepository identityRepository,
            ITokenService tokenService,
            IAuthenticationSessionService sessionService)
        {
            _identityRepository = identityRepository;
            _tokenService = tokenService;
            _sessionService = sessionService;
        }

        public async Task<LoginResult> LoginAsync(
            LoginRequestDto requestDto,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default)
        {
           var email = requestDto.Email.Trim();

            var user = await _identityRepository.FindByEmailAsync(email, cancellationToken);


            if(user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var passwordResult = await _identityRepository.CheckPasswordAsync(user, requestDto.Password, cancellationToken);

            if (passwordResult.IsLockedOut)
            {
                throw new UnauthorizedAccessException(
                    "The account is temporarily locked.");
            }
            if (passwordResult.IsNotAllowed)
            {
                throw new UnauthorizedAccessException(
                    "Email confirmation is required.");
            }

            if (!passwordResult.Succeeded)
            {
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");
            }
            var roles = await _identityRepository.GetRolesAsync(
                user,
                cancellationToken);

            var session = await _sessionService.CreateSessionAsync(
                user,
                ipAddress,
                userAgent,
                cancellationToken);

            var accessToken = _tokenService.GenerateAccessToken(
                user,
                roles,
                session.SessionId);

            return new LoginResult(
                new LoginResponseDto(
                    user.Id,
                    user.FullName,
                    user.Email!,
                    accessToken.Token,
                    accessToken.Expiration),
                session.RefreshToken,
                session.RefreshTokenExpiresAtUtc);
        }
    }
}
