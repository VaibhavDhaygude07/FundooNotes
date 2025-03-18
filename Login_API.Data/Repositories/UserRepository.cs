using FundooNotes.Data.Entity;
using Login_API.Data;
using Login_API.Data.Models;
using Login_API.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
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

        // ✅ Get User by Email (Fixed return type)
        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // ✅ Check if User Exists
        public bool CheckUserExists(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        // ✅ Get All Users
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // ✅ Get User by ID
        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // ✅ Update User
        public async Task UpdateUser(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser != null)
            {
                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.Email = user.Email;
                existingUser.PasswordHash = user.PasswordHash;

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
            }
        }

        // ✅ Delete User
        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
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
            string resetLink = $"https://localhost:4200/reset-password?email={email}&token={token}";
            string subject = "Password Reset Request";
            string body = $"<p>Click <a href='{resetLink}'>here</a> to reset your password.</p>";

            await SendEmailAsync(email, subject, body);
            return true;
        }

      

        public async Task<bool> ResetPassword(string email, string token, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.ResetToken != token || user.ResetTokenExpiry < DateTime.UtcNow)
                return false; // Invalid or expired token

            // ✅ Clean token before using
            token = token?.Trim().Replace("\n", "").Replace("\r", "");

            // ✅ Hash new password securely
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // ✅ Remove token after successful reset
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

        // ✅ Send Email using SMTP (Fixed visibility to `public`)
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("vaibhavdhaygude9077@gmail.com", "yrmw yghk fntc twru"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("vaibhavdhaygude9077@gmail.com"),
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
