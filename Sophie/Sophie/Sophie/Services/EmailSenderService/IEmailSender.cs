using System;
using System.IO;
using System.Threading.Tasks;

namespace Sophie.Services.EmailSenderService
{
    public interface IEmailSender
    {
        Task SendEmail(string email, string subject, string htmlContent, MemoryStream? fileStream = null, string? fileName = null);
    }
}
