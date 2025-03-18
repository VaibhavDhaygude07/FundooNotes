using FundooNotes.Data.Entity;
using Login_API.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Login_API.Data.Repositories
{
    public interface IUserRepository
    {
        Task<bool> RegisterUser(User user);
        Task<User> AuthenticateUser(string email, string password);
        Task<User> GetUserByEmail(string email);  // Ensure it's Task<User>
        bool CheckUserExists(string email);

        // Password Reset
        Task<bool> GeneratePasswordResetToken(string email, string token);
        Task<bool> ResetPassword(string email, string token, string newPassword);

        // User Management
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUserById(int id);
        Task UpdateUser(User user);
        Task DeleteUser(int id);

        // Send Email
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
