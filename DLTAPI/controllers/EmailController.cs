using AuthDemo.Data;
using DLTAPI.DTOs;
using DLTAPI.interfaces;
using DLTAPI.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;

namespace DLTAPI.controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailRepo _emailRepo;
        private readonly DataContext _context;

        public EmailController(IEmailRepo emailRepo, DataContext context, UserManager<ApplicationUser> userManager)
        {
            _emailRepo = emailRepo;
            _context = context;
            _userManager = userManager;
        }

        [HttpPost("send-verification-code")]
        [AllowAnonymous]
        public async Task<IActionResult> SendCode([FromBody] string toEmail)
        {
            await _emailRepo.SendVerificationCode(toEmail);
            return Ok(new { message = "Verification code sent!" });
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] CodeVerifyRequest req)
        {
            var success = await _emailRepo.VerifyCodeAsync(req.Email, req.Code);
            if (!success)
                return BadRequest("Invalid or expired code.");

            return Ok(new { message = "Email Verified" });
        }

        [HttpPost("send-password-reset-code")]
        [AllowAnonymous]
        public async Task<IActionResult> SendPasswordResetCode([FromBody] string email)
        {
            // Check if user exists
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // For security reasons, don't reveal if user doesn't exist
                return Ok(new { message = "If an account with this email exists, a reset code has been sent." });
            }

            // Generate and save code
            var code = new Random().Next(100000, 999999).ToString();

            var verificationCode = new VerificationCode
            {
                Email = email,
                Code = code,
                Type = VerificationType.PasswordReset,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            // Remove any existing reset codes for this email
            var existingCodes = await _context.VerificationCodes
                .Where(vc => vc.Email == email && vc.Type == VerificationType.PasswordReset)
                .ToListAsync();

            _context.VerificationCodes.RemoveRange(existingCodes);
            _context.VerificationCodes.Add(verificationCode);
            await _context.SaveChangesAsync();

            // Send email
            var subject = "Password Reset Code";
            var body = $"<h1>Your Password Reset Code: {code}</h1>" +
                    "<p>This code will expire in 10 minutes.</p>";

            await _emailRepo.SendEmailAsync(email, subject, body, true);

            return Ok("If an account with this email exists, a reset code has been sent.");
        }

        [HttpPost("verify-password-reset-code")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyPasswordResetCode([FromBody] CodeVerifyRequest req)
        {
            var codeRecord = await _context.VerificationCodes
                .Where(vc => vc.Email == req.Email &&
                            vc.Code == req.Code &&
                            vc.Type == VerificationType.PasswordReset)
                .OrderByDescending(vc => vc.ExpiresAt)
                .FirstOrDefaultAsync();

            if (codeRecord == null || codeRecord.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Invalid or expired code.");

            // Don't remove the code yet - we'll need it in the reset step
            // Just return a success response with a token that can be used to reset password
            return Ok(new
            {
                isValid = true,
                email = req.Email,
                code = req.Code
            });
        }

        [HttpPost("send-to-shota")]
        [AllowAnonymous]
        public async Task<IActionResult> SendMailToShota([FromBody] EmailDto email)
        {
            try
            {
                await _emailRepo.SendToShotaAsync(email);
                return Ok(new { message = "Email sent to Shota!" });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}