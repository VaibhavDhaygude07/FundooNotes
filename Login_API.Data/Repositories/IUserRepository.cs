using FundooNotes.Data.Entity;
using Login_API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login_API.Data.Repositories
{
    public interface IUserRepository
    {
        Task<bool> RegisterUser(User user);
        public Task<User> AuthenticateUser(string email, string password);


    }
}
