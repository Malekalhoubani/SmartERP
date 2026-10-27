using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Services.Models
{
    public sealed record AccessTokenResult(string Token, DateTimeOffset Expiration)
    {
    }
}
