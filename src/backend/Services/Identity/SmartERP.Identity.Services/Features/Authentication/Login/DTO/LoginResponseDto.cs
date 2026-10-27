using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Services.Features.Authentication.Login.DTO;

public sealed record LoginResponseDto(
    int UserId,
    string FullName,
    string Email,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAtUtc);
