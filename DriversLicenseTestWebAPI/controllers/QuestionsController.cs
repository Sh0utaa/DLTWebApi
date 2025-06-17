using DriversLicenseTestWebAPI.DTOs;
using DriversLicenseTestWebAPI.interfaces;
using DriversLicenseTestWebAPI.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DriversLicenseTestWebAPI.controllers
{
    [Route("api/questions")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IScrapeQuestions _scrapeQuestions;
        private readonly IQuestionRepo _questionRepo;
        private readonly UserManager<IdentityUser> _userManager;
        public QuestionsController(
            IScrapeQuestions scrapeQuestions,
            IQuestionRepo questionRepo,
            UserManager<IdentityUser> userManager)
        {
            _scrapeQuestions = scrapeQuestions;
            _questionRepo = questionRepo;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetQuestionsAsync()
        {
            var questions = await _questionRepo.GetQuestionsAsync();

            return Ok(questions);
        }

        [HttpGet("exam-questions")]
        [Authorize(Policy = "AuthenticatedUser")]
        public async Task<IActionResult> GetExamQuestions()
        {
            var questions = await _questionRepo.GetExamQuestions();
            List<QuestionDto> questionDtos = new List<QuestionDto>();

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

        [HttpGet("by-page/{index}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetQuestionsByPageIndexAsync(int index)
        {
            var questions = await _questionRepo.GetQuestionsWithAnswersByPageIndexAsync(index);

            return Ok(questions);
        }

        [HttpGet("by-id/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetQuestionById(int id)
        {
            var question = await _questionRepo.GetQuestionByIdAsync(id);

            return Ok(question);
        }

        [HttpGet("scrape")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ScrapeAllQuestionsAsync()
        {
            var questions = await _scrapeQuestions.ScrapeAllQuestionsAsync();
            return Ok(questions);
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