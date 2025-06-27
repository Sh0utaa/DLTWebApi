using DLTAPI.models;

namespace DLTAPI.interfaces
{
    public interface IScrapeQuestions
    {
        Task<List<List<Question>>> ScrapeAllQuestionsAsync(string language);
        Task<List<Question>> ScrapeCategoryAsync(int categoryId, string language);
    }
}