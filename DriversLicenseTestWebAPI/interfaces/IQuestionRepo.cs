

using DriversLicenseTestWebAPI.DTOs;
using DriversLicenseTestWebAPI.models;

namespace DriversLicenseTestWebAPI.interfaces
{
    public interface IQuestionRepo
    {
        Task<List<Question>> GetQuestionsAsync(int categoryId, string language);
        Task<List<Question>> GetAllQuestionsAsync();
        Task<List<Question>> GetExamQuestions(int categoryId, string language);
        Task<ExamSession> HandleExamSubmission(List<UserAnswerSubmissionDto> submissionDtos, string UserId);
    }
}