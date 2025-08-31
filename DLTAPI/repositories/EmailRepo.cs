using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;
using DLTAPI.models;
using DLTAPI.interfaces;
using MailKit.Security;
using AuthDemo.Data;
using DLTAPI.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DLTAPI.repositories
{
    public class EmailRepo : IEmailRepo
    {
        private readonly EmailSettings _settings;
        private readonly DataContext _context;

        public EmailRepo(IOptions<EmailSettings> settings, DataContext context)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _context = context;
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

        public async Task SendVerificationCode(string toEmail)
        {
            var code = new Random().Next(100000, 999999).ToString();

            var verificationCode = new VerificationCode
            {
                Email = toEmail,
                Code = code,
                Type = VerificationType.EmailConfirmation,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            _context.VerificationCodes.Add(verificationCode);
            await _context.SaveChangesAsync();

            var subject = "DLT Verification Code";
            var body = $"<h1> Your Verification Code: {code} </h1>";
            await SendEmailAsync(toEmail, subject, body, true);
        }

        public async Task SendToShotaAsync(EmailDto mail)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse("stevdorashvili08@gmail.com"));
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

        public async Task<bool> VerifyCodeAsync(string email, string code)
        {
            var record = await _context.VerificationCodes
           .Where(vc => vc.Email == email && vc.Code == code)
           .OrderByDescending(vc => vc.ExpiresAt)
           .FirstOrDefaultAsync();

            if (record == null || record.ExpiresAt < DateTime.UtcNow)
                return false;

            // Save verified state
            _context.VerifiedEmails.Add(new VerifiedEmail
            {
                Email = email,
                VerifiedAt = DateTime.UtcNow
            });

            _context.VerificationCodes.Remove(record);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsEmailVerifiedAsync(string email)
        {
            return await _context.VerifiedEmails.AnyAsync(v => v.Email == email);
        }

        public async Task ClearVerificationAsync(string email)
        {
            var record = await _context.VerifiedEmails.FirstOrDefaultAsync(v => v.Email == email);
            if (record != null)
            {
                _context.VerifiedEmails.Remove(record);
                await _context.SaveChangesAsync();
            }
        }
    }
}