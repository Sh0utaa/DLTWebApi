using DriversLicenseTestWebAPI.models;

namespace DriversLicenseTestWebAPI.interfaces
{
    public interface IScrapeQuestions
    {
        Task<List<Question>> ScrapeAllQuestions();
    }
}