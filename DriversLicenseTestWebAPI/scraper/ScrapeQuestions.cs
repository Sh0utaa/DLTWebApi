using DriversLicenseTestWebAPI.interfaces;
using DriversLicenseTestWebAPI.models;

namespace DriversLicenseTestWebAPI.scraper
{
    public class ScrapeQuestions : IScrapeQuestions
    {
        public async Task<List<Question>> ScrapeAllQuestions()
        {
            try
            {
                var questions = new List<Question>();

                return questions;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}