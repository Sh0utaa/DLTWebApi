
namespace DLTAPI.DTOs
{
    public class AnswerResultDto
    {
        public int QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public string Explanation { get; set; }
    }
}