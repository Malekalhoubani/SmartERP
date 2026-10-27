using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SmartERP.Identity.DataAccess.Options;
using SmartERP.Identity.Services.Interfaces;

namespace SmartERP.Identity.DataAccess.Services;

public sealed class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;

    public EmailService(IOptions<EmailOptions> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }

    public async Task SendAsync(
        string recipientEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(
            _emailOptions.SenderName,
            _emailOptions.SenderEmail));
        message.To.Add(MailboxAddress.Parse(recipientEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var smtpClient = new SmtpClient
        {
            Timeout = (int)TimeSpan
                .FromSeconds(_emailOptions.TimeoutSeconds)
                .TotalMilliseconds
        };

        var securityOption = _emailOptions.UseStartTls
            ? SecureSocketOptions.StartTls
            : SecureSocketOptions.None;

        await smtpClient.ConnectAsync(
            _emailOptions.SmtpHost,
            _emailOptions.SmtpPort,
            securityOption,
            cancellationToken);

        if (_emailOptions.UseAuthentication)
        {
            await smtpClient.AuthenticateAsync(
                _emailOptions.Username,
                _emailOptions.Password,
                cancellationToken);
        }

        await smtpClient.SendAsync(message, cancellationToken);
        await smtpClient.DisconnectAsync(true, cancellationToken);
    }
}
