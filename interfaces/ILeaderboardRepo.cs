

using DLTAPI.DTOs;

namespace DLTAPI.interfaces
{
    public interface ILeaderboardRepo
    {
        Task<List<LeaderboardEntryDto>> GetExamPassRateLeaderboardAsync();
    }
}