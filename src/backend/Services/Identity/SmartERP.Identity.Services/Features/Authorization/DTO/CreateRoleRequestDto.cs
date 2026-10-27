using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Services.Features.Authorization.DTO
{
    public sealed record CreateRoleRequestDto(string Name, string? Description)
    {
    }
}
