
namespace DriversLicenseTestWebAPI.models
{
    public class ExamSession
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CorrectAmount { get; set; }
        public int IncorrectAmount { get; set; }
        public bool Failed { get; set; }
        public List<UserAnswerSubmission> Answers { get; set; } = new List<UserAnswerSubmission>();
    }
}