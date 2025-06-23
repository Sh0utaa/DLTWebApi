namespace DriversLicenseTestWebAPI.interfaces
{
    public interface IEmailRepo
    {
        Task SendEmailAsync(string to, string subject, string body, bool html);
    }
}