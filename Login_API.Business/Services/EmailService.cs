using FundooNotes.Business.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Your Name", "vdhaygude2002@gmail.com"));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = "Password Reset";
        email.Body = new TextPart("plain") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync("vdhaygude2002@gmail.com", "tloe whhq vwpf awdw"); // Use App Password
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
