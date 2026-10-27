using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Domain.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAtUtc { get; set; } =DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAtUtc { get; set; }
        public ICollection<UserSession> Sessions { get; set; } =new List<UserSession>();
    }
}
