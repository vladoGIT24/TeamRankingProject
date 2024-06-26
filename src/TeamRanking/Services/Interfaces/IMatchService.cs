﻿using TeamRanking.Persistence.Entity;
using TeamRanking.Models;

namespace TeamRanking.Services.Interfaces
{
    public interface IMatchService
    {
        Task<IEnumerable<MatchDto>> GetAllMatchesAsync();
        Task<MatchDto> GetMatchByIdAsync(int id);
        Task<MatchDto> CreateMatchAsync(CreateMatchDto createMatchDto);
        Task<MatchDto> UpdateMatchAsync(int id, UpdateMatchDto updateMatchDto);
        Task DeleteMatchAsync(int id);
    }
}
