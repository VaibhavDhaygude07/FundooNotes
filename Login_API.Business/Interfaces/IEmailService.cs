using System.Threading.Tasks;

namespace FundooNotes.Business.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string body);
        //Task SendEmailAsync(string email, string resetLink);
    }

}
