

namespace DLTAPI.models
{
    public class Question()
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int PageIndex { get; set; }
        public string? Image { get; set; }
        public string QuestionContent { get; set; }
        public string Language { get; set; }
        public List<Answer?> Answers { get; set; }
        public void ValidateAnswers()
        {
            if (Answers[0] == null || Answers[1] == null)
                throw new InvalidOperationException("The frist two answers cannot be null");

            if (Answers == null || Answers.Count < 2)
                throw new ArgumentException("At least two answers are required");

            if (Answers.Count > 4)
                throw new InvalidOperationException("Answers must contain exactly 4 elements");
        }
    }
}