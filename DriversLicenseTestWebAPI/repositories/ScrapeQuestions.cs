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

        public async Task<List<Question>> ScrapeAllQuestions()
        {
            var questions = new List<Question>();

            for (int i = 1; i <= 91; i++)
            {
                var url = $"https://teoria.on.ge/tickets/0?page={i}";

                try
                {
                    var httpClient = new HttpClient();
                    var html = await httpClient.GetStringAsync(url);

                    var config = Configuration.Default.WithDefaultLoader();
                    var context = BrowsingContext.New(config);
                    var document = await context.OpenAsync(req => req.Content(html));

                    var ticketContainers = document.QuerySelectorAll(".ticket-container");

                    if (ticketContainers.Length == 0)
                    {
                        Console.WriteLine($"No tickets found on page {i}");
                        continue;
                    }

                    foreach (var container in ticketContainers)
                    {
                        var question = new Question
                        {
                            PageIndex = i,
                            Image = container.QuerySelector(".t-image img")?.GetAttribute("src"),
                            QuestionContent = container.QuerySelector(".t-question-inner")?.TextContent?.Trim(),
                            Answers = new List<Answer>()
                        };

                        var answerElements = container.QuerySelectorAll(".t-answer:not(.ans-empty)");
                        for (int j = 0; j < answerElements.Length; j++)
                        {
                            question.Answers.Add(new Answer
                            {
                                Text = CleanText(answerElements[j].TextContent),
                                IsCorrect = answerElements[j].HasAttribute("data-is-correct-list"),
                            });
                        }

                        // Check if question already exists in database
                        var exists = await _context.Questions
                            .AnyAsync(q => q.QuestionContent == question.QuestionContent);

                        if (!exists)
                        {
                            await _context.Questions.AddAsync(question);
                            await _context.SaveChangesAsync();
                            questions.Add(question);
                        }
                        else
                        {
                            Console.WriteLine($"Question already exists: {question.QuestionContent}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error scraping page {i}: {ex.Message}");
                    continue;
                }
            }

            return questions;
        }
    }
}