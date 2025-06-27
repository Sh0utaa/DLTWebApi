

namespace DLTAPI.DTOs
{
    public class ExamSessionDto
    {

        public int CorrectAmount { get; set; }
        public int IncorrectAmount { get; set; }
        public bool Failed { get; set; }
    }
}