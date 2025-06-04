using DriversLicenseTestWebAPI.interfaces;
using DriversLicenseTestWebAPI.models;
using HtmlAgilityPack;

namespace DriversLicenseTestWebAPI.scraper
{
    public class ScrapeQuestions : IScrapeQuestions
    {
        public async Task<List<Question>> ScrapeAllQuestions()
        {
            try
            {
                var questions = new List<Question>();
                var url = "https://teoria.on.ge/tickets/0?page=1";
                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(url);


                return questions;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}