
using DLTAPI.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DLTAPI.controllers
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
        public async Task<IActionResult> ScrapeAllQuestions([FromQuery] string language = "ka")
        {
            if (language != "ka" && language != "en")
            {
                return BadRequest("Invalid language. Must be 'ka' or 'en'.");
            }

            var questions = await _scrapeQuestions.ScrapeAllQuestionsAsync(language);
            return Ok(questions);
        }

        [HttpGet("{categoryId:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ScrapeCategory(int categoryId, [FromQuery] string language = "ka")
        {
            if (categoryId < 1 || categoryId > 10)
            {
                return BadRequest("Invalid category ID. Must be between 1 and 10.");
            }

            if (language != "ka" && language != "en")
            {
                return BadRequest("Invalid language. Must be 'ka' or 'en'.");
            }

            var questions = await _scrapeQuestions.ScrapeCategoryAsync(categoryId, language);
            return Ok(questions);
        }
    }

}