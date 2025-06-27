using DLTAPI.models;
using Microsoft.AspNetCore.Identity;

namespace DLTAPI.Helper
{
    public static class HelperFunctions
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            const string adminEmail = "shota.tevdorashvili@gau.edu.ge";
            const string adminPassword = "Admin123";

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "Shotesko",
                    FirstName = "Shota",
                    LastName = "Tevdorashvili",
                    DateOfBirth = new DateOnly(2006, 12, 12),
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
