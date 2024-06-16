using System.Text.Json;
using TeamRanking.Models;
using TeamRanking.Services.Interfaces;

namespace TeamRanking.Services
{
    public class FileReaderService : IFileReaderService
    {
        public async Task<List<CreateUpdateTeamDto>> ReadTeamsFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The JSON file was not found.", filePath);
            }

            var jsonData = await File.ReadAllTextAsync(filePath);
            var createUpdateTeamDtos = JsonSerializer.Deserialize<List<CreateUpdateTeamDto>>(jsonData);

            if (createUpdateTeamDtos == null || createUpdateTeamDtos.Count == 0)
            {
                throw new InvalidDataException("Invalid team data.");
            }

            return createUpdateTeamDtos;
        }
    }
}
