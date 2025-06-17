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

            input = Regex.Replace(input, @"^\d+\s*", "");

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

        public async Task<List<List<Question>>> ScrapeAllQuestionsAsync()
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
                { 10, 17 }
            };

            var questions = new List<List<Question>>();

            foreach (var kvp in pagesDict)
            {
                var result = await ScrapeCategoryAsync(kvp.Key, kvp.Value);
                questions.Add(result);
            }
            return questions;
        }

        private async Task<List<Question>> ScrapeCategoryAsync(int category, int pages)
        {
            var questions = new List<Question>();
            using var httpClient = new HttpClient();

            for (int i = 1; i <= pages; i++)
            {
                var url = $"https://teoria.on.ge/tickets/{category}?page={i}";

                try
                {
                    var html = await httpClient.GetStringAsync(url);

                    var config = Configuration.Default.WithDefaultLoader();
                    var context = BrowsingContext.New(config);
                    var document = await context.OpenAsync(req => req.Content(html));

                    var ticketContainers = document.QuerySelectorAll(".ticket-container");

                    if (ticketContainers.Length == 0)
                    {
                        Console.WriteLine($"No tickets found on category {category}, page {i}");
                        continue;
                    }

                    foreach (var container in ticketContainers)
                    {
                        var questionContent = container.QuerySelector(".t-question-inner")?.TextContent?.Trim();

                        // Skip if question content is empty
                        if (string.IsNullOrWhiteSpace(questionContent))
                        {
                            Console.WriteLine($"Empty question content found on category {category}, page {i}");
                            continue;
                        }

                        // Check if question already exists in database first
                        var exists = await _context.Questions
                            .AnyAsync(q => q.CategoryId == category && q.QuestionContent == questionContent);

                        if (exists)
                        {
                            Console.WriteLine($"Question already exists: {questionContent}");
                            continue;
                        }

                        var question = new Question
                        {
                            PageIndex = i,
                            CategoryId = category,
                            Image = container.QuerySelector(".t-image img")?.GetAttribute("src"),
                            QuestionContent = questionContent,
                            Answers = new List<Answer>()
                        };

                        var answerElements = container.QuerySelectorAll(".t-answer:not(.ans-empty)");
                        foreach (var answerElement in answerElements)
                        {
                            question.Answers.Add(new Answer
                            {
                                Text = CleanText(answerElement.TextContent),
                                IsCorrect = answerElement.HasAttribute("data-is-correct-list"),
                            });
                        }

                        await _context.Questions.AddAsync(question);
                        questions.Add(question);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"HTTP error while scraping category {category}, page {i}: {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error happened while scraping category {category}, page {i}: {ex.Message}");
                }

                await Task.Delay(1000);
            }

            return questions;
        }
    }
}