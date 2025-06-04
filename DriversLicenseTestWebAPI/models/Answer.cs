

namespace DriversLicenseTestWebAPI.models
{
    public class Answer
    {
        public int id { get; set; }
        public int questionId { get; set; }
        public string text { get; set; } = "";
        public bool isCorrect { get; set; }

        public Question? question { get; set; }
    }
}