using DLTAPI.DTOs;
using DLTAPI.interfaces;
using DLTAPI.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DLTAPI.controllers
{
    [Route("api/questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionRepo _questionRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public QuestionsController(
            IQuestionRepo questionRepo,
            UserManager<ApplicationUser> userManager)
        {
            _questionRepo = questionRepo;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetQuestionsAsync([FromQuery] string language)
        {
            var questions = await _questionRepo.GetAllQuestionsAsync();

            return Ok(questions);
        }

        [HttpGet("{categoryId:int}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetQuestionsByCategory(int categoryId, [FromQuery] string language = "ka")
        {
            if (language != "ka" && language != "en")
            {
                return BadRequest("Invalid language. Must be 'ka' or 'en'.");
            }

            var questions = await _questionRepo.GetQuestionsAsync(categoryId, language);

            return Ok(questions);
        }

        [HttpGet("exam/{categoryId:int}")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetExamQuestionsByCategory(int categoryId, [FromQuery] string language = "ka")
        {
            if (language != "ka" && language != "en")
            {
                return BadRequest("Invalid language. Must be 'ka' or 'en'.");
            }

            var questions = await _questionRepo.GetExamQuestions(categoryId, language);
            var questionDtos = new List<QuestionDto>();

            foreach (var question in questions)
            {
                var answerDtos = question.Answers.Select(a => new AnswerDto
                {
                    Id = a.Id,
                    Text = a.Text
                }).ToList();

                var questionDto = new QuestionDto()
                {
                    Id = question.Id,
                    QuestionContent = question.QuestionContent,
                    Image = question.Image,
                    Answers = answerDtos
                };

                questionDtos.Add(questionDto);
            }

            return Ok(questionDtos);
        }

        [HttpPost("submit")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> SubmitAnswers([FromBody] List<UserAnswerSubmissionDto> submissionDtos)
        {
            string UserId = _userManager.GetUserId(User);

            ExamSession examSession = await _questionRepo.HandleExamSubmission(submissionDtos, UserId);

            var result = new ExamSessionDto()
            {
                CorrectAmount = examSession.CorrectAmount,
                IncorrectAmount = examSession.IncorrectAmount,
                Failed = examSession.Failed
            };

            return Ok(result);
        }
    }
}