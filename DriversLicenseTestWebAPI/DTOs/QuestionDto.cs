

namespace DriversLicenseTestWebAPI.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string QuestionContent { get; set; }
        public List<AnswerDto?> Answers { get; set; } = new();
    }
}