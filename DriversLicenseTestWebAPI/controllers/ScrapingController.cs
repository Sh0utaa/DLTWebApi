
using DriversLicenseTestWebAPI.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriversLicenseTestWebAPI.controllers
{
    [Route("api/scrape")]
    [ApiController]

    public class ScrapingController : ControllerBase
    {
        private readonly IScrapeQuestions _scrapeQuestions;

        public ScrapingController(IScrapeQuestions scrapeQuestions)
        {
            _scrapeQuestions = scrapeQuestions;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ScrapeAllQuestions()
        {
            var questions = await _scrapeQuestions.ScrapeAllQuestionsAsync();
            return Ok(questions);
        }

        [HttpGet("{categoryId:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ScrapeCategory(int categoryId)
        {
            if (categoryId < 1 || categoryId > 10)
            {
                return BadRequest("Invalid category ID. Must be between 1 and 10.");
            }

            var questions = await _scrapeQuestions.ScrapeCategoryAsync(categoryId);
            return Ok(questions);
        }
    }

}