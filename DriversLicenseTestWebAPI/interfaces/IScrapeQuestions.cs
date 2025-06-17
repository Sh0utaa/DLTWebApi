using DriversLicenseTestWebAPI.models;

namespace DriversLicenseTestWebAPI.interfaces
{
    public interface IScrapeQuestions
    {
        Task<List<List<Question>>> ScrapeAllQuestionsAsync();
        Task<List<Question>> ScrapeCategoryAsync(int categoryId);
    }
}