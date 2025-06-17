using DriversLicenseTestWebAPI.models;

namespace DriversLicenseTestWebAPI.interfaces
{
    public interface IScrapeQuestions
    {
        Task<List<List<Question>>> ScrapeAllQuestionsAsync();
        Task<List<Question>> GetCategoryOneQuestions();
        Task<List<Question>> GetCategoryTwoQuestions();
        Task<List<Question>> GetCategoryThreeQuestions();
        Task<List<Question>> GetCategoryFourQuestions();
        Task<List<Question>> GetCategoryFiveQuestions();
        Task<List<Question>> GetCategorySixQuestions();
        Task<List<Question>> GetCategorySevenQuestions();
        Task<List<Question>> GetCategoryEightQuestions();
        Task<List<Question>> GetCategoryNineQuestions();
        Task<List<Question>> GetCategoryTenQuestions();
    }
}