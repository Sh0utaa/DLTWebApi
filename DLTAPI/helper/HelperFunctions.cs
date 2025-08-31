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
            const string adminPassword = "Admin!123__";

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

            const string publicEmail = "generic@generic";
            const string publicPassword = "Generic!123__";

            var publicUser = await userManager.FindByEmailAsync(publicEmail);
            if (publicUser == null)
            {
                publicUser = new ApplicationUser
                {
                    UserName = "Public",
                    DateOfBirth = new DateOnly(2000, 1, 1),
                    Email = publicEmail,
                    EmailConfirmed = true,
                };
                var result = await userManager.CreateAsync(publicUser, publicPassword);

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

        public static string ParseRailwayConnectionString(string railwayUrl)
        {
            if (railwayUrl.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(railwayUrl);
                var userInfo = uri.UserInfo.Split(':');
                var user = userInfo[0];
                var password = userInfo.Length > 1 ? userInfo[1] : "";
                
                return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
            }
            return railwayUrl;
        }
    }
}
