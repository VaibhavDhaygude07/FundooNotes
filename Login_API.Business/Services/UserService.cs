using BCrypt.Net;
using FundooNotes.Data.Entity;
using Login_API.Business.Interfaces;
using Login_API.Data.Models;
using Login_API.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login_API.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterUser(string firstName, string lastName, string email, string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { FirstName = firstName, LastName = lastName, Email = email, PasswordHash = hashedPassword };
            return await _userRepository.RegisterUser(user);
        }

        public async Task<User> Authenticate(string email, string password)
        {
            var user = await _userRepository.AuthenticateUser( email,  password);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return null;
            return user;
        }
    }
}
