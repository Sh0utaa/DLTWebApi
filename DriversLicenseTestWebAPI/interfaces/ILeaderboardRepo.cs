

using DriversLicenseTestWebAPI.DTOs;

namespace DriversLicenseTestWebAPI.interfaces
{
    public interface ILeaderboardRepo
    {
        Task<List<LeaderboardEntryDto>> GetExamPassRateLeaderboardAsync();
    }
}