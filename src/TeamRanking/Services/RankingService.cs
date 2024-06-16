using Microsoft.EntityFrameworkCore;
using TeamRanking.Persistence;
using TeamRanking.Persistence.Entity;
using TeamRanking.Services.Interfaces;
using TeamRanking.Services.Models;

namespace TeamRanking.Services
{
    public class RankingService : IRankingService
    {
        private readonly ApplicationDbContext _context;

        public RankingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RankingDto>> GetAllRankingsAsync()
        {
            return await _context.Ranking.Include(r => r.Team)
                .OrderByDescending(r => r.Points)
                .Select(r => new RankingDto
                {
                    TeamId = r.TeamId,
                    TeamName = r.Team.TeamName,
                    Points = r.Points,
                    PlayedGames = r.PlayedGames,
                    Victories = r.Victories,
                    Draws = r.Draws,
                    Defeats = r.Defeats
                })
                .ToListAsync();
        }

        public async Task<RankingDto> GetRankingByTeamIdAsync(int teamId)
        {
            var ranking = await _context.Ranking.Include(r => r.Team)
                .FirstOrDefaultAsync(r => r.TeamId == teamId);
            if (ranking == null)
            {
                return null;
            }
            return new RankingDto
            {
                TeamId = ranking.TeamId,
                TeamName = ranking.Team.TeamName,
                Points = ranking.Points,
                PlayedGames = ranking.PlayedGames,
                Victories = ranking.Victories,
                Draws = ranking.Draws,
                Defeats = ranking.Defeats
            };
        }

        public async Task UpdateRankingsAsync(Match match)
        {
            var homeTeamRanking = await _context.Ranking.FirstOrDefaultAsync(r => r.TeamId == match.HomeTeamId);
            var awayTeamRanking = await _context.Ranking.FirstOrDefaultAsync(r => r.TeamId == match.AwayTeamId);

            if (homeTeamRanking != null && awayTeamRanking != null)
            {
                homeTeamRanking.PlayedGames++;
                awayTeamRanking.PlayedGames++;

                if (match.HomeTeamScore > match.AwayTeamScore)
                {
                    homeTeamRanking.Victories++;
                    homeTeamRanking.Points += (int)Scoring.Win;
                    awayTeamRanking.Defeats++;
                }
                else if (match.HomeTeamScore < match.AwayTeamScore)
                {
                    awayTeamRanking.Victories++;
                    awayTeamRanking.Points += (int)Scoring.Win;
                    homeTeamRanking.Defeats++;
                }
                else
                {
                    homeTeamRanking.Draws++;
                    awayTeamRanking.Draws++;
                    homeTeamRanking.Points += (int)Scoring.Draw;
                    awayTeamRanking.Points += (int)Scoring.Draw;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task InitializeRankingForTeamAsync(int teamId)
        {
            var ranking = new Ranking
            {
                TeamId = teamId,
                Points = 0,
                PlayedGames = 0,
                Victories = 0,
                Draws = 0,
                Defeats = 0
            };

            _context.Ranking.Add(ranking);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRankingByTeamIdAsync(int teamId)
        {
            var ranking = await _context.Ranking.FirstOrDefaultAsync(r => r.TeamId == teamId);
            if (ranking != null)
            {
                _context.Ranking.Remove(ranking);
                await _context.SaveChangesAsync();
            }
        }
    }
}