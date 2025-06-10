using System.Text.RegularExpressions;
using AngleSharp;
using AuthDemo.Data;
using DriversLicenseTestWebAPI.interfaces;
using DriversLicenseTestWebAPI.models;
using Microsoft.EntityFrameworkCore;

namespace DriversLicenseTestWebAPI.repositories
{
    public class ScrapeQuestions : IScrapeQuestions
    {
        private readonly DataContext _context;
        private readonly IQuestionRepo _questionRepo;
        public ScrapeQuestions(DataContext context, IQuestionRepo questionRepo)
        {
            _context = context;
            _questionRepo = questionRepo;
        }

        private static string CleanText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            return Regex.Replace(input, @"\s+", " ").Trim();
        }

        public Task<List<Question>> GetCategoryEightQuestions()
        {
            throw new NotImplementedException();
        }

        public Task<List<Question>> GetCategoryFiveQuestions()
        {
            throw new NotImplementedException();
        }

        public Task<List<Question>> GetCategoryFourQuestions()
        {
            throw new NotImplementedException();
        }

        public Task<List<Question>> GetCategoryNineQuestions()
        {
            throw new NotImplementedException();
        }

        public Task<List<Question>> GetCategoryOneQuestions()
        {
            throw new NotImplementedException();
        }

        public Task<List<Question>> GetCategorySevenQuestions()
        {
            throw new NotImplementedException();
        }

        public Task<List<Question>> GetCategorySixQuestions()
        {
            throw new NotImplementedException();
        }

        public Task<List<Question>> GetCategoryTenQuestions()
        {
            throw new NotImplementedException();
        }

        public Task<List<Question>> GetCategoryThreeQuestions()
        {
            throw new NotImplementedException();
        }

        public Task<List<Question>> GetCategoryTwoQuestions()
        {
            throw new NotImplementedException();
        }

        // public async Task<List<Question>> ScrapeAllQuestions()
        // {
        //     var questions = new List<Question>();

        //     for (int i = 1; i <= 91; i++)
        //     {
        //         var url = $"https://teoria.on.ge/tickets/0?page={i}";

        //         try
        //         {
        //             var httpClient = new HttpClient();
        //             var html = await httpClient.GetStringAsync(url);

        //             var config = Configuration.Default.WithDefaultLoader();
        //             var context = BrowsingContext.New(config);
        //             var document = await context.OpenAsync(req => req.Content(html));

        //             var ticketContainers = document.QuerySelectorAll(".ticket-container");

        //             if (ticketContainers.Length == 0)
        //             {
        //                 Console.WriteLine($"No tickets found on page {i}");
        //                 continue;
        //             }

        //             foreach (var container in ticketContainers)
        //             {
        //                 var question = new Question
        //                 {
        //                     PageIndex = i,
        //                     Image = container.QuerySelector(".t-image img")?.GetAttribute("src"),
        //                     QuestionContent = container.QuerySelector(".t-question-inner")?.TextContent?.Trim(),
        //                     Answers = new List<Answer>()
        //                 };

        //                 var answerElements = container.QuerySelectorAll(".t-answer:not(.ans-empty)");
        //                 for (int j = 0; j < answerElements.Length; j++)
        //                 {
        //                     question.Answers.Add(new Answer
        //                     {
        //                         Text = CleanText(answerElements[j].TextContent),
        //                         IsCorrect = answerElements[j].HasAttribute("data-is-correct-list"),
        //                     });
        //                 }

        //                 // Check if question already exists in database
        //                 var exists = await _context.Questions
        //                     .AnyAsync(q => q.QuestionContent == question.QuestionContent);

        //                 if (!exists)
        //                 {
        //                     await _context.Questions.AddAsync(question);
        //                     await _context.SaveChangesAsync();
        //                     questions.Add(question);
        //                 }
        //                 else
        //                 {
        //                     Console.WriteLine($"Question already exists: {question.QuestionContent}");
        //                 }
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             Console.WriteLine($"Error scraping page {i}: {ex.Message}");
        //             continue;
        //         }
        //     }

        //     return questions;
        // }



        public async Task<List<Question>> ScrapeAllQuestionsAsync()
        {
            var pagesDict = new Dictionary<int, int>
            {
                { 1, 44 },
                { 2, 17 },
                { 3, 55 },
                { 4, 55 },
                { 5, 54 },
                { 6, 50 },
                { 7, 49 },
                { 8, 52 },
                { 9, 50 },
                { 10, 73 }
            };

            var questions = new List<Question>();
            var tasks = new List<Task<List<Question>>>();

            foreach (var (category, pageCount) in pagesDict)
            {
                tasks.Add(ScrapeCategoryAsync(category, pageCount));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var categoryResults in results)
            {
                questions.AddRange(categoryResults);
            }

            return questions;
        }

        private async Task<List<Question>> ScrapeCategoryAsync(int category, int pages)
        {
            // Implementation to be added later
            return new List<Question>();
        }
    }
}