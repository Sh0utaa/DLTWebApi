using DriversLicenseTestWebAPI.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DriversLicenseTestWebAPI.controllers
{
    [Route("api/questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IScrapeQuestions _scrapeQuestions;
        private readonly IQuestionRepo _questionRepo;
        public QuestionsController(IScrapeQuestions scrapeQuestions, IQuestionRepo questionRepo)
        {
            _scrapeQuestions = scrapeQuestions;
            _questionRepo = questionRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetQuestionsAsync()
        {
            var questions = await _questionRepo.GetQuestionsAsync();

            return Ok(questions);
        }

        [HttpGet("by-page/{index}")]
        public async Task<IActionResult> GetQuestionsByPageIndexAsync(int index)
        {
            var questions = await _questionRepo.GetQuestionsWithAnswersByPageIndexAsync(index);

            return Ok(questions);
        }

        [HttpGet("scrape")]
        public async Task<IActionResult> ScrapeQuestionsAsync()
        {
            var questions = await _scrapeQuestions.ScrapeAllQuestions();
            return Ok(questions);
        }
    }
}