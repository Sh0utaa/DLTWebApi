using DLTAPI.DTOs;

namespace DLTAPI.interfaces
{
    public interface IEmailRepo
    {
        Task SendEmailAsync(string to, string subject, string body, bool html);
        Task SendVerificationCode(string toEmail);
        Task<bool> VerifyCodeAsync(string email, string code);
        Task<bool> IsEmailVerifiedAsync(string email);
        Task ClearVerificationAsync(string email);
        Task SendToShotaAsync(EmailDto email);
    }
}