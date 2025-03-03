using FundooNotes.Data.Entity;
using Login_API.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BCrypt.Net;

namespace Login_API.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        // ✅ Register User with Secure Password Hashing
        public async Task<bool> RegisterUser(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return false; // Email already exists

            var newUser = new User
            {
                Email = user.Email,
                PasswordHash = HashPassword(user.PasswordHash),
                ResetToken = null,
                ResetTokenExpiry = null
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Authenticate User
        public async Task<User> AuthenticateUser(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null; // Invalid credentials

            return user;
        }

        // ✅ Generate Reset Token & Send Email
        public async Task<bool> GeneratePasswordResetToken(string email, string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // 🔹 Send Reset Email
            string resetLink = $"https://yourdomain.com/reset-password?email={email}&token={token}";
            string subject = "Password Reset Request";
            string body = $"<p>Click <a href='{resetLink}'>here</a> to reset your password.</p>";

            await SendEmailAsync(email, subject, body);
            return true;
        }

        // ✅ Reset Password
        public async Task<bool> ResetPassword(string email, string token, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.ResetToken != token || user.ResetTokenExpiry < DateTime.UtcNow)
                return false; // Invalid or expired token

            user.PasswordHash = HashPassword(newPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // ✅ Secure Password Hashing using BCrypt
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // ✅ Verify Password
        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        // ✅ Send Email using SMTP
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("vdhaygude2002@gmail.com", "hqcm imsf eulx zbbv"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("vdhaygude2002@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }
    }
}
