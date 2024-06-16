using TeamRanking.Persistence.Entity;
using TeamRanking.Services.Models;

namespace TeamRanking.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamDto>> GetAllTeamsAsync();
        Task<TeamDto> GetTeamByIdAsync(int id);
        Task<TeamDto> CreateTeamAsync(CreateUpdateTeamDto createUpdateTeamDto);
        Task<TeamDto> UpdateTeamAsync(int id, CreateUpdateTeamDto createUpdateTeamDto);
        Task DeleteTeamAsync(int id);
    }
}
