

using DriversLicenseTestWebAPI.models;

namespace DriversLicenseTestWebAPI.interfaces
{
    public interface IQuestionRepo
    {
        Task<List<Question>> GetQuestionsAsync();
        Task<List<Question>> GetQuestionsByPageIndexAsync(int index);
        Task<List<Question>> GetQuestionsWithAnswersByPageIndexAsync(int index);
    }
}