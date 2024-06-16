using TeamRanking.Models;

namespace TeamRanking.Services.Interfaces
{
    public interface IFileReaderService
    {
        Task<List<CreateUpdateTeamDto>> ReadTeamsFromFileAsync(string filePath);
    }
}
