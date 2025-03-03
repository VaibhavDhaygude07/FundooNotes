﻿using System.Threading.Tasks;

namespace FundooNotes.Business.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail,  string body);
    }
}
