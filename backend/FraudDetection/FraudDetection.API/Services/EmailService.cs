using MailKit.Net.Smtp;
using MimeKit;

namespace FraudDetection.API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration configuration)
    {
        _config = configuration;
    }

    public async Task SendEmailAsync(string toEmail,string subject,string body)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(
                    _config["EmailSettings:SenderName"],
                    _config["EmailSettings:SenderEmail"]
        ));

        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new TextPart("html")
        {
            Text = body
        };

        using var smpt = new SmtpClient();

        await smpt.ConnectAsync(
               _config["EmailSettings:SmtpServer"],
            int.Parse(_config["EmailSettings:Port"]!),
            MailKit.Security.SecureSocketOptions.StartTls
        );

        await smpt.AuthenticateAsync(_config["EmailSettings:Username"],
            _config["EmailSettings:Password"]);

        await smpt.SendAsync(email);
        await smpt.DisconnectAsync(true);

    }
}