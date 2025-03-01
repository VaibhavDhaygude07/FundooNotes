using FundooNotes.Data.Entity;
using Login_API.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Login_API.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }

        // Register User
        public async Task<bool> RegisterUser(User user)
        {
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return false; // Email already exists

            var newUser = new User
            {
                Email = user.Email,
                PasswordHash = HashPassword(user.PasswordHash), // Ensure password is hashed
                ResetToken = null,  // Explicitly setting to NULL
                ResetTokenExpiry = null
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return true;
        }

        // Authenticate User
        public async Task<User> AuthenticateUser(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Generate Reset Token
        public async Task<bool> GeneratePasswordResetToken(string email, string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Reset Password
        public async Task<bool> ResetPassword(string email, string token, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.ResetToken == token);
            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow) return false; // Invalid or expired token

            user.PasswordHash = HashPassword(newPassword);
            user.ResetToken = null; // Clear the reset token after use
            user.ResetTokenExpiry = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Hashing Password (Simple example, replace with a proper hashing method like BCrypt)
        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password)); // Placeholder hashing
        }
    }
}
