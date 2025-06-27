using DLTAPI.DTOs;

namespace DLTAPI.interfaces
{
    public interface IEmailRepo
    {
        Task SendEmailAsync(string to, string subject, string body, bool html);
        Task SendToShotaAsync(EmailDto email);
    }
}