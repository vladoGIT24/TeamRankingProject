﻿using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TeamRanking.Persistence;
using TeamRanking.Persistence.Entity;
using TeamRanking.Services.Interfaces;
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

        public async Task DeleteTeamAsync(int teamId)
        {
            var team = await _context.Teams.FindAsync(teamId);
            if (team != null)
            {
                await _rankingService.DeleteRankingByTeamIdAsync(teamId);
                team.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task BulkCreateTeamsFromFileAsync(string filePath)
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

            var existingTeams = await _context.Teams
                                              .Where(t => createUpdateTeamDtos.Select(dto => dto.Name).Contains(t.TeamName))
                                              .ToListAsync();

            var newTeams = createUpdateTeamDtos
                .Where(dto => !existingTeams.Any(et => et.TeamName == dto.Name))
                .Select(dto => new Team
                {
                    TeamName = dto.Name,
                    IsDeleted = false
                }).ToList();

            _context.Teams.AddRange(newTeams);
            await _context.SaveChangesAsync();

            foreach (var team in newTeams)
            {
                await _rankingService.InitializeRankingForTeamAsync(team.TeamId);
            }
        }
    }
}
