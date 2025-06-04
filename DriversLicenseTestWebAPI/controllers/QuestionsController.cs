using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DriversLicenseTestWebAPI.controllers
{
    [Route("api/questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetQuestionsAsync()
        {
            var questions = await Task.Run(() =>
            {
                return new List<string>
                {
                    "What is the speed limit in a residential area?",
                    "When should you yield at a four-way stop?",
                    "What does a flashing yellow light mean?"
                };
            });

            return Ok();
        }

        [HttpGet("validateAnswers")]
        public async Task<IActionResult> ValidateAnswers()
        {
            return Ok();
        }

        [HttpGet("scrape")]
        public async Task<IActionResult> ScrapeQuestionsAsync()
        {
            return Ok();
        }
    }
}