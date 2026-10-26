using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartERP.Identity.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string recipientEmail, string subject, string htmlBody, CancellationToken cancellationToken = default);
    }
}
