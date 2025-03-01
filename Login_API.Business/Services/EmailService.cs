using FundooNotes.Business.Interfaces;
using Microsoft.Extensions.Configuration;
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

    public async Task<bool> SendEmailAsync(string toEmail, string resetLink)
    {
        try
        {
            string fromEmail = _configuration["EmailSettings:FromEmail"];
            string smtpServer = _configuration["EmailSettings:SmtpServer"];
            int smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            string smtpUser = _configuration["EmailSettings:SmtpUser"];
            string smtpPassword = _configuration["EmailSettings:SmtpPassword"];

            Console.WriteLine($"Attempting to send email to: {toEmail}");
            Console.WriteLine($"SMTP Server: {smtpServer}");
            Console.WriteLine($"SMTP Port: {smtpPort}");
            Console.WriteLine($"From Email: {fromEmail}");
            Console.WriteLine($"SMTP User: {smtpUser}");

            using (MailMessage mailMessage = new MailMessage(fromEmail, toEmail))
            {
                mailMessage.Subject = "Password Reset Request";
                mailMessage.Body = $@"
            <p>You requested a password reset. Click the link below to reset your password:</p>
            <p><a href='{resetLink}' target='_blank'>Reset Password</a></p>
            <p>If you did not request this, please ignore this email.</p>";
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = Encoding.UTF8;

                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(smtpUser, smtpPassword);
                    smtpClient.Timeout = 30000; // 30 seconds timeout


                    await smtpClient.SendMailAsync(mailMessage);
                    Console.WriteLine("✅ Email sent successfully!");
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

