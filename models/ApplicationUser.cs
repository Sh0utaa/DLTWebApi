
using Microsoft.AspNetCore.Identity;

namespace DLTAPI.models
{
    public class ApplicationUser : IdentityUser
    {
        public DateOnly? DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}