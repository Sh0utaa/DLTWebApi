

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
    }
}