using System.Text.Json;
using System.Text.RegularExpressions;
using AngleSharp;
using AuthDemo.Data;
using DLTAPI.interfaces;
using DLTAPI.models;
using Microsoft.EntityFrameworkCore;

namespace DLTAPI.repositories
{
    public class ScrapeQuestions : IScrapeQuestions
    {
        private readonly DataContext _context;
        public ScrapeQuestions(DataContext context)
        {
            _context = context;
        }
        Dictionary<int, int> pagesDict = new Dictionary<int, int>
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

        private static string CleanText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            input = Regex.Replace(input, @"^\d+\s*", "");

            return Regex.Replace(input, @"\s+", " ").Trim();
        }

        public async Task<List<List<Question>>> ScrapeAllQuestionsAsync(string language)
        {
            var questions = new List<List<Question>>();

            foreach (var kvp in pagesDict)
            {
                var result = await ScrapeCategoryAsync(kvp.Key, kvp.Value, language);
                questions.Add(result);
            }
            return questions;
        }
        public Task<List<Question>> ScrapeCategoryAsync(int categoryId, string language)
        {
            if (!pagesDict.ContainsKey(categoryId))
            {
                throw new ArgumentException("Invalid categoryId");
            }

            return ScrapeCategoryAsync(categoryId, pagesDict[categoryId], language);
        }

        private async Task<List<Question>> ScrapeCategoryAsync(int category, int pages, string language)
        {
            var questions = new List<Question>();
            using var httpClient = new HttpClient();

            var cookieObj = new
            {
                category = 2,
                locale = language,
                skin = "dark",
                user = 0,
                created = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            var cookieJson = JsonSerializer.Serialize(cookieObj);
            var encodedCookie = Uri.EscapeDataString(cookieJson);
            httpClient.DefaultRequestHeaders.Add("Cookie", $"exam-settings={encodedCookie}");

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
                            Answers = new List<Answer>(),
                            Language = language
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