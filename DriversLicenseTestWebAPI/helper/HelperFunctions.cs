using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DriversLicenseTestWebAPI.Helper
{
    public static class HelperFunctions
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin@123";

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        public class DummyEmailSender : IEmailSender<IdentityUser>
        {
            public Task SendConfirmationLinkAsync(IdentityUser user, string email, string confirmationLink)
                => Task.CompletedTask;

            public Task SendPasswordResetLinkAsync(IdentityUser user, string email, string resetLink)
                => Task.CompletedTask;

            public Task SendPasswordResetCodeAsync(IdentityUser user, string email, string resetCode)
                => Task.CompletedTask;
        }
    }
}
