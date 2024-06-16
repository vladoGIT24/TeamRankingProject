﻿using Microsoft.AspNetCore.Mvc;
using TeamRanking.Interfaces;
using TeamRanking.Persistence.Entity;
using TeamRanking.Services.Models;

namespace TeamRanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchDto>>> GetMatches()
        {
            var matches = await _matchService.GetAllMatchesAsync();
            return Ok(matches);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDto>> GetMatch(int id)
        {
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null)
            {
                return NotFound();
            }
            return Ok(match);
        }

        [HttpPost]
        public async Task<ActionResult<MatchDto>> CreateMatch(CreateMatchDto createMatchDto)
        {
            var match = await _matchService.CreateMatchAsync(createMatchDto);
            return CreatedAtAction(nameof(GetMatch), new { id = match.MatchId }, match);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatch(int id, UpdateMatchDto updateMatchDto)
        {
            var match = await _matchService.UpdateMatchAsync(id, updateMatchDto);
            if (match == null)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null)
            {
                return NotFound();
            }
            await _matchService.DeleteMatchAsync(id);
            return NoContent();
        }
    }
}