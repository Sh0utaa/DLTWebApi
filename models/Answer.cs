

using System.Text.Json.Serialization;

namespace DLTAPI.models
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; } = "";
        public bool IsCorrect { get; set; }

        [JsonIgnore]
        public Question? Question { get; set; }
    }
}