using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using DriversLicenseTestWebAPI.models;
using DriversLicenseTestWebAPI.interfaces;
using MailKit.Security;
using AuthDemo.Data;
using DriversLicenseTestWebAPI.DTOs;

namespace DriversLicenseTestWebAPI.repositories
{
    public class EmailRepo : IEmailRepo
    {
        private readonly EmailSettings _settings;

        public EmailRepo(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool html)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            if (html)
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };
            else
                email.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendToShotaAsync(EmailDto mail)
        {

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse("shota@shotatevdorashvili.com"));
            email.Subject = mail.Subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = $"Name: {mail.Name}\nEmail: {mail.Mail}\n\nMessage:\n{mail.Body}"
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}