namespace DriversLicenseTestWebAPI.models
{
    public class VerificationCode
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpiresAt { get; set; }
        public VerificationType Type { get; set; }

    }

    public enum VerificationType
    {
        EmailConfirmation,
        PasswordReset
    }
}