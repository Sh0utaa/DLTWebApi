namespace DLTAPI.DTOs
{
    public class RegisterDto
    {
        public string UserName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}