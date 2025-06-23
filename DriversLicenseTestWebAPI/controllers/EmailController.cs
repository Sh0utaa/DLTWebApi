using AuthDemo.Data;
using DriversLicenseTestWebAPI.interfaces;
using DriversLicenseTestWebAPI.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriversLicenseTestWebAPI.controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailRepo _emailRepo;
        private readonly DataContext _context;

        public EmailController(IEmailRepo emailRepo, DataContext context)
        {
            _emailRepo = emailRepo;
            _context = context;
        }

        [HttpPost("send-verification-code")]
        public async Task<IActionResult> SendCode([FromBody] string toEmail)
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
            await _emailRepo.SendEmailAsync(toEmail, subject, body, true);
            return Ok("Verification code sent!");
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] CodeVerifyRequest req)
        {
            var codeRecord = await _context.VerificationCodes
                .Where(vc => vc.Email == req.Email && vc.Code == req.Code)
                .OrderByDescending(vc => vc.ExpiresAt)
                .FirstOrDefaultAsync();

            if (codeRecord == null || codeRecord.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Invalid or expired code.");

            _context.VerificationCodes.Remove(codeRecord);
            await _context.SaveChangesAsync();

            return Ok("Code verified");
        }


        [HttpPost("send-test")]
        public async Task<IActionResult> SendTestEmail([FromBody] string toEmail)
        {
            var subject = "Test Email";
            var body = "This is a test email from your .NET app.";
            await _emailRepo.SendEmailAsync(toEmail, subject, body, false);
            return Ok("Test email sent.");
        }
    }
}