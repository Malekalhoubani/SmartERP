using SmartERP.Identity.Services.Features.Authentication.Login.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Services.Features.Authentication.Login
{
    public interface ILoginService
    {
        Task<Models.LoginResult> LoginAsync(
            LoginRequestDto loginRequestDto,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default);
    }
}
