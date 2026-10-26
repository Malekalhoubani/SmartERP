using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Domain.Entities
{
    public sealed class ApplicationRole : IdentityRole<int>
    {
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    }
}
