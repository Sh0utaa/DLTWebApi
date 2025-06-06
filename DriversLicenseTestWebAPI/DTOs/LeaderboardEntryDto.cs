
namespace DriversLicenseTestWebAPI.DTOs
{
    public class LeaderboardEntryDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public int TotalSessions { get; set; }
        public int PassedSessions { get; set; }
        public int FailedSessions { get; set; }
        public double PassRate { get; set; }
    }
}