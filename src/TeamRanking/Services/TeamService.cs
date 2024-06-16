using Microsoft.EntityFrameworkCore;
using TeamRanking.Interfaces;
using TeamRanking.Persistence;
using TeamRanking.Persistence.Entity;
using TeamRanking.Services.Models;

namespace TeamRanking.Services
{
    public class TeamService : ITeamService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRankingService _rankingService;

        public TeamService(ApplicationDbContext context, IRankingService rankingService)
        {
            _context = context;
            _rankingService = rankingService;   
        }

        public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync()
        {
            return await _context.Teams
                .Where(t => !t.IsDeleted)
                .Select(t => new TeamDto
                {
                    TeamId = t.TeamId,
                    Name = t.TeamName
                })
                .ToListAsync();
        }

        public async Task<TeamDto> GetTeamByIdAsync(int id)
        {
            var team = await _context.Teams
                .Where(t => !t.IsDeleted && t.TeamId == id)
                .FirstOrDefaultAsync();
            if (team == null)
            {
                return null;
            }
            return new TeamDto
            {
                TeamId = team.TeamId,
                Name = team.TeamName
            };
        }

        public async Task<TeamDto> CreateTeamAsync(CreateUpdateTeamDto createUpdateTeamDto)
        {
            var team = new Team
            {
                TeamName = createUpdateTeamDto.Name,
                IsDeleted = false
            };

            _context.Teams.Add(team);
            await _context.SaveChangesAsync();

            await _rankingService.InitializeRankingForTeamAsync(team.TeamId);

            return new TeamDto
            {
                TeamId = team.TeamId,
                Name = team.TeamName
            };
        }

        public async Task<TeamDto> UpdateTeamAsync(int id, CreateUpdateTeamDto createUpdateTeamDto)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team == null || team.IsDeleted)
            {
                return null;
            }

            team.TeamName = createUpdateTeamDto.Name;

            await _context.SaveChangesAsync();

            return new TeamDto
            {
                TeamId = team.TeamId,
                Name = team.TeamName
            };
        }

        public async Task DeleteTeamAsync(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                team.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
