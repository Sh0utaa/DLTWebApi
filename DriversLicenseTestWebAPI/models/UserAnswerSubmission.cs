

namespace DriversLicenseTestWebAPI.models
{
    public class UserAnswerSubmission
    {
        public int UserId { get; set; }
        public int questionId { get; set; }
        public int answerId { get; set; }
    }
}