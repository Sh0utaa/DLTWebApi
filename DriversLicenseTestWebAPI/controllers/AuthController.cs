using DriversLicenseTestWebAPI.DTOs;
using DriversLicenseTestWebAPI.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DriversLicenseTestWebAPI.controllers
{
    [Route("api/auth")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly SignInManager<ApplicationUser> _signinManager;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManger = userManager;
            _signinManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid login data.");
            try
            {
                var user = await _userManger.FindByEmailAsync(loginDto.Email);

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

    }
}