

using AuthDemo.Data;
using DriversLicenseTestWebAPI.interfaces;
using DriversLicenseTestWebAPI.models;
using Microsoft.EntityFrameworkCore;

namespace DriversLicenseTestWebAPI.repositories
{
    public class QuestionRepo : IQuestionRepo
    {
        private readonly DataContext _context;

        public QuestionRepo(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Question>> GetExamQuestions()
        {
            try
            {
                return await _context.Questions
                    .Include(q => q.Answers)
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

        public async Task<Question?> GetQuestionByIdAsync(int id)
        {
            try
            {
                return await _context.Questions.Include(q => q.Answers).FirstOrDefaultAsync(q => q.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting question by ID {id}: {ex}");

                throw;
            }
        }

        public async Task<List<Question>> GetQuestionsAsync()
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

        public async Task<List<Question>> GetQuestionsByPageIndexAsync(int index)
        {
            try
            {
                return await _context.Questions.Where(question => question.PageIndex == index).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error happened while getting questions by index: {ex}");

                throw;
            }
        }

        public async Task<List<Question>> GetQuestionsWithAnswersByPageIndexAsync(int index)
        {
            try
            {
                return await _context.Questions
                    .Include(q => q.Answers)
                    .Where(q => q.PageIndex == index)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error happened while getting questions with answers by index: {ex}");

                throw;
            }
        }

        public async Task<ExamSession> HandleExamSubmission(List<UserAnswerSubmission> submissions)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error happened while handiling submissions: {ex}");

                throw;
            }
        }
    }
}