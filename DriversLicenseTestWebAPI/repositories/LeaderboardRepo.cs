

using AuthDemo.Data;
using DriversLicenseTestWebAPI.DTOs;
using DriversLicenseTestWebAPI.interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DriversLicenseTestWebAPI.repositories
{
    public class LeaderboardRepo : ILeaderboardRepo
    {
        private readonly DataContext _context;
        private readonly UserManager<IdentityUser> _userManger;
        public LeaderboardRepo(DataContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManger = userManager;
        }

        public async Task<List<LeaderboardEntryDto>> GetExamPassRateLeaderboardAsync()
        {
            try
            {
                var groupedResults = await _context.ExamSessions
                    .GroupBy(es => es.UserId)
                    .Select(group => new LeaderboardEntryDto
                    {
                        UserId = group.Key,
                        TotalSessions = group.Count(),
                        PassedSessions = group.Count(es => !es.Failed),
                        FailedSessions = group.Count(es => es.Failed),
                        PassingRate = Math.Round((double)group.Count(es => !es.Failed) / group.Count() * 100, 3)
                    })
                    .OrderByDescending(x => x.PassingRate)
                    .ThenByDescending(x => x.PassedSessions)
                    .ToListAsync();

                var leaderboard = new List<LeaderboardEntryDto>();

                foreach (var entry in groupedResults)
                {
                    var user = await _userManger.FindByIdAsync(entry.UserId);
                    leaderboard.Add(new LeaderboardEntryDto
                    {
                        UserId = entry.UserId,
                        Email = user?.Email ?? "Unknown",
                        TotalSessions = entry.TotalSessions,
                        PassedSessions = entry.PassedSessions,
                        FailedSessions = entry.FailedSessions,
                        PassingRate = entry.PassingRate
                    });
                }

                return leaderboard;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling getting leaderboards: {ex}");

                throw;
            }
        }
    }
}