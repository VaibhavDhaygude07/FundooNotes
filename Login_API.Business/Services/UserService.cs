using BCrypt.Net;
using FundooNotes.Business.Interfaces;
using FundooNotes.Data.Entity;
using Login_API.Business.Interfaces;
using Login_API.Data.Models;
using Login_API.Data.Repositories;
using ModelLayer.Models;
using System;
using System.Threading.Tasks;

namespace Login_API.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService; // Injected Auth Service

        public UserService(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        // ✅ Register User
        public async Task<bool> RegisterUser(string firstName, string lastName, string email, string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PasswordHash = hashedPassword
            };
            return await _userRepository.RegisterUser(user);
        }

        // ✅ Authenticate User
        public async Task<User> Authenticate(string email, string password)
        {
            var user = await _userRepository.AuthenticateUser(email, password);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;
            return user;
        }

        // ✅ Forget Password (Fixed async issue & added token generation)
        public async Task<string> ForgetPassword(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
                throw new Exception("User does not exist");

            string newToken = _authService.GenerateToken(user.Id, email, true); // Generate reset token
            bool tokenSaved = await _userRepository.GeneratePasswordResetToken(email, newToken);
            return tokenSaved ? newToken : null;
        }

        // ✅ Reset Password
        public async Task<bool> ResetPassword(string email, string token, string newPassword)
        {
            return await _userRepository.ResetPassword(email, token, newPassword);
        }

    }
}
