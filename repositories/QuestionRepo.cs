

using AuthDemo.Data;
using DLTAPI.DTOs;
using DLTAPI.interfaces;
using DLTAPI.models;
using Microsoft.EntityFrameworkCore;

namespace DLTAPI.repositories
{
    public class QuestionRepo : IQuestionRepo
    {
        private readonly DataContext _context;

        public QuestionRepo(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Question>> GetExamQuestions(int categoryId, string language)
        {
            try
            {
                return await _context.Questions
                    .Include(q => q.Answers)
                    .Where(q => q.CategoryId == categoryId && q.Language == language)
                    .OrderBy(q => Guid.NewGuid())
                    .Take(30)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting random questions: {ex}");
                throw;
            }
        }
        public async Task<List<Question>> GetAllQuestionsAsync()
        {
            try
            {
                return await _context.Questions
                    .Include(q => q.Answers)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error happened while getting questions: {ex}");

                throw;
            }
        }

        public async Task<List<Question>> GetQuestionsAsync(int categoryId, string language)
        {
            try
            {
                return await _context.Questions
                    .Include(q => q.Answers)
                    .Where(q => q.CategoryId == categoryId && q.Language == language)
                    .OrderBy(q => Guid.NewGuid())
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting questions: {ex}");
                throw;
            }
        }

        public async Task<ExamSession> HandleExamSubmission(List<UserAnswerSubmissionDto> submissionDtos, string UserId)
        {
            try
            {
                if (submissionDtos == null || !submissionDtos.Any())
                    throw new ArgumentException("No submissions provided");

                List<UserAnswerSubmission> submissions = new List<UserAnswerSubmission>();

                foreach (var submissionDto in submissionDtos)
                {
                    var sub = new UserAnswerSubmission()
                    {
                        UserId = UserId,
                        QuestionId = submissionDto.QuestionId,
                        AnswerId = submissionDto.AnswerId,
                        IsCorrect = false
                    };

                    submissions.Add(sub);
                    // await _context.UserAnswerSubmissions.AddAsync(sub);
                }

                int CorrectAmount = 0, IncorrectAmount = 0;

                var questionList = await GetAllQuestionsAsync();
                var allQuestions = new HashSet<Question>(questionList);

                foreach (var submission in submissions)
                {
                    Question question = allQuestions.FirstOrDefault(q => q.Id == submission.QuestionId);

                    if (question == null)
                    {
                        Console.WriteLine($"Question with ID {submission.QuestionId} not found");
                        IncorrectAmount++;
                        continue;
                    }

                    if (question.Answers == null || !question.Answers.Any())
                    {
                        Console.WriteLine($"No answers found for question with ID {submission.QuestionId}");

                        IncorrectAmount++;
                        continue;
                    }

                    var selectedAnswer = question.Answers.FirstOrDefault(a => a.Id == submission.AnswerId);

                    if (selectedAnswer == null)
                    {
                        Console.WriteLine($"Selected answer with ID {submission.AnswerId} doesn't exist for question with ID {submission.QuestionId}");
                        IncorrectAmount++;
                    }
                    else if (selectedAnswer.IsCorrect)
                    {
                        CorrectAmount++;
                        submission.IsCorrect = true;
                    }
                    else
                    {
                        IncorrectAmount++;
                    }

                }

                await _context.UserAnswerSubmissions.AddRangeAsync(submissions);

                bool Failed = IncorrectAmount > 3;

                ExamSession examSession = new ExamSession()
                {
                    UserId = UserId,
                    CorrectAmount = CorrectAmount,
                    IncorrectAmount = IncorrectAmount,
                    Failed = Failed,
                    Answers = submissions
                };

                await _context.ExamSessions.AddAsync(examSession);
                await _context.SaveChangesAsync();

                return examSession;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error happened while handiling submissions: {ex}");

                throw;
            }
        }

    }
}