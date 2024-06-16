using TeamRanking.Persistence.Entity;
using TeamRanking.Services.Models;

namespace TeamRanking.Interfaces
{
    public interface IRankingService
    {
        Task<IEnumerable<RankingDto>> GetAllRankingsAsync();
        Task<RankingDto> GetRankingByTeamIdAsync(int teamId);
        Task InitializeRankingForTeamAsync(int teamId);
        Task UpdateRankingsAsync(Match match);
    }
}