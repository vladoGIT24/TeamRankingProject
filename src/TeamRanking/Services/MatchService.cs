using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamRanking.Persistence;
using TeamRanking.Persistence.Entity;
using TeamRanking.Services.Interfaces;
using TeamRanking.Services.Models;

namespace TeamRanking.Services
{
    public class MatchService : IMatchService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRankingService _rankingService;

        public MatchService(ApplicationDbContext context, IRankingService rankingService)
        {
            _context = context;
            _rankingService = rankingService;
        }

        public async Task<IEnumerable<MatchDto>> GetAllMatchesAsync()
        {
            return await _context.Matches
                .Select(m => new MatchDto
                {
                    MatchId = m.MatchId,
                    HomeTeamId = m.HomeTeamId,
                    AwayTeamId = m.AwayTeamId,
                    HomeTeamScore = m.HomeTeamScore,
                    AwayTeamScore = m.AwayTeamScore,
                    IsOver = m.IsOver
                })
                .ToListAsync();
        }

        public async Task<MatchDto> GetMatchByIdAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return null;
            }
            return new MatchDto
            {
                MatchId = match.MatchId,
                HomeTeamId = match.HomeTeamId,
                AwayTeamId = match.AwayTeamId,
                HomeTeamScore = match.HomeTeamScore,
                AwayTeamScore = match.AwayTeamScore,
                IsOver = match.IsOver
            };
        }

        public async Task<MatchDto> CreateMatchAsync(CreateMatchDto createMatchDto)
        {

            var homeTeamExists = await _context.Teams.AnyAsync(t => t.TeamId == createMatchDto.HomeTeamId && !t.IsDeleted);
            if (!homeTeamExists)
            {
                throw new InvalidOperationException($"Home team with ID {createMatchDto.HomeTeamId} does not exist.");
            }

            var awayTeamExists = await _context.Teams.AnyAsync(t => t.TeamId == createMatchDto.AwayTeamId && !t.IsDeleted);
            if (!awayTeamExists)
            {
                throw new InvalidOperationException($"Away team with ID {createMatchDto.AwayTeamId} does not exist.");
            }

            var match = new Match
            {
                HomeTeamId = createMatchDto.HomeTeamId,
                AwayTeamId = createMatchDto.AwayTeamId,
                HomeTeamScore = createMatchDto.HomeTeamScore,
                AwayTeamScore = createMatchDto.AwayTeamScore,
                IsOver = createMatchDto.IsOver
            };

            _context.Matches.Add(match);
            await _context.SaveChangesAsync();

            if (match.IsOver)
            {
                await _rankingService.UpdateRankingsAsync(match);
            }

            return new MatchDto
            {
                MatchId = match.MatchId,
                HomeTeamId = match.HomeTeamId,
                AwayTeamId = match.AwayTeamId,
                HomeTeamScore = match.HomeTeamScore,
                AwayTeamScore = match.AwayTeamScore,
                IsOver = match.IsOver
            };
        }
        public async Task<MatchDto> UpdateMatchAsync(int id, UpdateMatchDto updateMatchDto)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return null;
            }

            if (match.IsOver)
            {
                throw new InvalidOperationException("The match is over and cannot be updated.");
            }

            match.HomeTeamScore = updateMatchDto.HomeTeamScore;
            match.AwayTeamScore = updateMatchDto.AwayTeamScore;
            match.IsOver = updateMatchDto.IsOver;

            await _context.SaveChangesAsync();

            if (match.IsOver)
            {
                await _rankingService.UpdateRankingsAsync(match);
            }

            return new MatchDto
            {
                MatchId = match.MatchId,
                HomeTeamId = match.HomeTeamId,
                AwayTeamId = match.AwayTeamId,
                HomeTeamScore = match.HomeTeamScore,
                AwayTeamScore = match.AwayTeamScore,
                IsOver = match.IsOver
            };
        }

        public async Task DeleteMatchAsync(int id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match != null)
            {
                _context.Matches.Remove(match);
                await _context.SaveChangesAsync();
            }
        }
    }
}