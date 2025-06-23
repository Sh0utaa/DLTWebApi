
using Microsoft.AspNetCore.Identity;

namespace DriversLicenseTestWebAPI.models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}