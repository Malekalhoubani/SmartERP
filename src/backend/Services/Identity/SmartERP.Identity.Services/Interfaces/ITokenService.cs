using SmartERP.Identity.Domain.Entities;
using SmartERP.Identity.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Services.Interfaces
{
    public interface ITokenService
    {
        AccessTokenResult GenerateAccessToken(
            ApplicationUser user,
            IReadOnlyCollection<string> roles,
            long sessionId);
    }
}
