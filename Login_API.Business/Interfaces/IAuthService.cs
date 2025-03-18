using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotes.Business.Interfaces
{
    public interface IAuthService
    {
        //string GenerateToken(int userId, string email, bool isPasswordReset = false);
        //bool ValidateResetToken(string token, out string email);

        string GenerateToken(int userId, string email, bool isResetToken = false);
        bool ValidateResetToken(string token, out string email);


    }
}
