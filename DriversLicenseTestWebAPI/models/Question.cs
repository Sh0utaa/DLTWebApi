

using DriversLicenseTestWebAPI.DTOs;

namespace DriversLicenseTestWebAPI.models
{
    public class Question()
    {
        public int id { get; set; }
        public string? image { get; set; }
        public string question { get; set; }
        public List<AnswerDto?> answers { get; set; }
        public void ValidateAnswers()
        {
            if (answers[0] == null || answers[1] == null)
                throw new InvalidOperationException("The frist two answers cannot be null");

            if (answers == null || answers.Count < 2)
                throw new ArgumentException("At least two answers are required");

            if (answers.Count > 4)
                throw new InvalidOperationException("Answers must contain exactly 4 elements");
        }
    }
}