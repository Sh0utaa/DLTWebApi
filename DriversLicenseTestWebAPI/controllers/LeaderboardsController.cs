using DriversLicenseTestWebAPI.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DriversLicenseTestWebAPI.controllers
{
    [Route("api/leaderboards")]
    [ApiController]
    public class LeaderboardsController : ControllerBase
    {
        private readonly ILeaderboardRepo _leaderboardRepo;
        public LeaderboardsController(ILeaderboardRepo leaderboardRepo)
        {
            _leaderboardRepo = leaderboardRepo;
        }

        [HttpGet("/rates")]
        public async Task<IActionResult> GetExamPassRates()
        {
            try
            {
                var results = await _leaderboardRepo.GetExamPassRateLeaderboardAsync();

                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Problem accured in Leaderboard Controller: {ex}");

                throw;
            }
        }
    }
}