

using System.Text.Json.Serialization;

namespace DriversLicenseTestWebAPI.models
{
    public class UserAnswerSubmission
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public int SessionId { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.Now;


        [JsonIgnore]
        public Question Question { get; set; }

        [JsonIgnore]
        public ExamSession ExamSession { get; set; }

        [JsonIgnore]
        public Answer Answer { get; set; }
    }
}