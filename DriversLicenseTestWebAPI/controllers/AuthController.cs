using System.Security.Claims;
using AuthDemo.Data;
using DriversLicenseTestWebAPI.DTOs;
using DriversLicenseTestWebAPI.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriversLicenseTestWebAPI.controllers
{
    [Route("api/auth")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly DataContext _context;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
            _signinManager = signInManager;
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
                    return Ok("Login successful");
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
                return BadRequest("Invalid login data.");

            try
            {
                var existingEmail = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingEmail != null)
                    return BadRequest("Email exists already");

                var user = new ApplicationUser
                {
                    UserName = registerDto.UserName,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    DateOfBirth = registerDto.DateOfBirth,
                    Email = registerDto.Email,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return BadRequest(errors);
                }

                return Ok("User registered successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while registering user: ", ex);
                throw;
            }
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
    }
}