

namespace DriversLicenseTestWebAPI.DTOs
{
    public class QuestionDto
    {
        public int id { get; set; }
        public string image { get; set; }
        public string question { get; set; }
        public List<AnswerDto> AnswerOptions { get; set; } = new();
    }
}