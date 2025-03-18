using FundooNotes.Data.Entity;
using Login_API.Data.Models;
using ModelLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login_API.Business.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterUser(string firstName, string lastName, string email, string password);
        Task<User> Authenticate(string email, string password);
        Task<string> ForgetPassword(string email);
        Task<bool> ResetPassword(string email, string token, string newPassword);
    }
}
