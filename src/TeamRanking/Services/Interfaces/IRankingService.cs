using TeamRanking.Persistence.Entity;
using TeamRanking.Services.Models;

namespace TeamRanking.Services.Interfaces
{
    public interface IRankingService
    {
        Task<IEnumerable<RankingDto>> GetAllRankingsAsync();
        Task<RankingDto> GetRankingByTeamIdAsync(int teamId);
        Task InitializeRankingForTeamAsync(int teamId);
        Task UpdateRankingsAsync(Match match);
        Task DeleteRankingByTeamIdAsync(int teamId);
    }
}