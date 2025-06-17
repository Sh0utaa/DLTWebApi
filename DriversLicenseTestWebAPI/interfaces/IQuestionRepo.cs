

using DriversLicenseTestWebAPI.DTOs;
using DriversLicenseTestWebAPI.models;

namespace DriversLicenseTestWebAPI.interfaces
{
    public interface IQuestionRepo
    {
        Task<List<Question>> GetQuestionsAsync(int categoryId);
        Task<List<Question>> GetAllQuestionsAsync();
        Task<List<Question>> GetExamQuestions(int categoryId);
        Task<ExamSession> HandleExamSubmission(List<UserAnswerSubmissionDto> submissionDtos, string UserId);
    }
}