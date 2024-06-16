using Microsoft.AspNetCore.Mvc;
using TeamRanking.Persistence.Entity;
using TeamRanking.Services.Interfaces;
using TeamRanking.Models;

namespace TeamRanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankingController : ControllerBase
    {
        private readonly IRankingService _rankingService;

        public RankingController(IRankingService rankingService)
        {
            _rankingService = rankingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RankingDto>>> GetRankings()
        {
            var rankings = await _rankingService.GetAllRankingsAsync();

            return Ok(rankings);
        }

        [HttpGet("{teamId}")]
        public async Task<ActionResult<RankingDto>> GetRankingByTeamId(int teamId)
        {
            var ranking = await _rankingService.GetRankingByTeamIdAsync(teamId);
            if (ranking == null)
            {
                return NotFound(new { Message = "Ranking not found for team", TeamId = teamId });
            }
            return Ok(ranking);
        }
    }
}