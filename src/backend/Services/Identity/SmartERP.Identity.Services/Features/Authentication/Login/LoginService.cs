using SmartERP.Identity.Repositories.Interfaces;
using SmartERP.Identity.Services.Features.Authentication.Login.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Services.Features.Authentication.Login
{
     public sealed class LoginService : ILoginService
    {
        private readonly IIdentityRepository _identityRepository;
        public LoginService(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto requestDto, CancellationToken cancellationToken = default)
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

            return new LoginResponseDto(
            user.Id,
            user.FullName,
            user.Email!);
        }
    }
}
