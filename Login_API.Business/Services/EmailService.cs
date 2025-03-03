using FundooNotes.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(string toEmail,  string body)
    {
        try
        {
            string fromEmail = _configuration["EmailSettings:FromEmail"];
            string smtpServer = _configuration["EmailSettings:SmtpServer"];
            int smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            string smtpUser = _configuration["EmailSettings:SmtpUser"];
            string smtpPassword = _configuration["EmailSettings:SmtpPassword"];

            using (var mailMessage = new MailMessage(fromEmail, toEmail))
            {
                
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = Encoding.UTF8;

                using (var smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);

                    await smtpClient.SendMailAsync(mailMessage);
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Email sending failed: {ex.Message}");
            return false;
        }
    }
}
