using System.Security.Claims;
using AuthDemo.Data;
using DLTAPI.DTOs;
using DLTAPI.interfaces;
using DLTAPI.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DLTAPI.controllers
{
    [Route("api/auth")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly IEmailRepo _emailRepo;
        private readonly DataContext _context;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DataContext context, IEmailRepo emailRepo)
        {
            _userManager = userManager;
            _context = context;
            _signinManager = signInManager;
            _emailRepo = emailRepo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid login data.");
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null)
                    return Unauthorized("Invalid email or password.");

                var result = await _signinManager.PasswordSignInAsync(user.UserName, loginDto.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Ok(new { message = "Login successful" });
                }
                else if (result.IsLockedOut)
                {
                    return Forbid("Account is locked.");
                }
                else
                {
                    return Unauthorized("Invalid email or password.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "An error occurred during login.");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid registration data.");

            var existingEmail = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingEmail != null)
                return BadRequest("Email already exists.");

            var isVerified = await _emailRepo.IsEmailVerifiedAsync(registerDto.Email);
            if (!isVerified)
                return BadRequest("Email is not verified. Please verify it before registering.");

            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                DateOfBirth = registerDto.DateOfBirth,
                Email = registerDto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(errors);
            }

            await _emailRepo.ClearVerificationAsync(registerDto.Email);

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signinManager.SignOutAsync();
                return Ok(new { message = "user logged out successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpGet("validate-user")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> ValidateUser()
        {
            try
            {
                bool isAuthenticated = User.Identity?.IsAuthenticated ?? false;

                return Ok(new { isValid = isAuthenticated });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        [HttpPost("confirm-email")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> ConfirmEmail([FromBody] string code)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var codeRecord = await _context.VerificationCodes
               .Where(vc => vc.Email == email &&
                           vc.Code == code &&
                           vc.Type == VerificationType.EmailConfirmation)
               .OrderByDescending(vc => vc.ExpiresAt)
               .FirstOrDefaultAsync();

            if (codeRecord == null || codeRecord.ExpiresAt < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Invalid or expired verification code." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.EmailConfirmed = true;
            }

            _context.VerificationCodes.Remove(codeRecord);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Email confirmed successfully." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            // Validate the code first
            var codeRecord = await _context.VerificationCodes
                .Where(vc => vc.Email == resetDto.Email &&
                            vc.Code == resetDto.Code &&
                            vc.Type == VerificationType.PasswordReset)
                .OrderByDescending(vc => vc.ExpiresAt)
                .FirstOrDefaultAsync();

            if (codeRecord == null || codeRecord.ExpiresAt < DateTime.UtcNow)
                return BadRequest("Invalid or expired reset code.");

            // Find the user
            var user = await _userManager.FindByEmailAsync(resetDto.Email);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Reset the password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, resetDto.NewPassword);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            // Remove the used code
            _context.VerificationCodes.Remove(codeRecord);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password has been reset successfully." });
        }
    }
}