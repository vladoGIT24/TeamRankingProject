using Microsoft.AspNetCore.Mvc;
using TeamRanking.Services.Interfaces;
using TeamRanking.Services.Models;

namespace TeamRanking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public TeamsController(ITeamService teamService, IWebHostEnvironment hostingEnvironment)
        {
            _teamService = teamService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetTeams()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDto>> GetTeam(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return Ok(team);
        }

        [HttpPost]
        public async Task<ActionResult<TeamDto>> CreateTeam(CreateUpdateTeamDto createUpdateTeamDto)
        {
            var team = await _teamService.CreateTeamAsync(createUpdateTeamDto);
            return CreatedAtAction(nameof(GetTeam), new { id = team.TeamId }, team);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, CreateUpdateTeamDto createUpdateTeamDto)
        {
            var team = await _teamService.UpdateTeamAsync(id, createUpdateTeamDto);
            if (team == null)
            {
                return NotFound(new { Message = "Team not found", TeamId = id });
            }
            return Ok(new { Message = "Team updated successfully", Data = team });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
            {
                return NotFound(new { Message = "Team not found", TeamId = id });
            }
            await _teamService.DeleteTeamAsync(id);
            return Ok(new { Message = "Team deleted successfully", TeamId = id });
        }

        [HttpPost("import")]
        public async Task<IActionResult> BulkCreateTeamsFromFile()
        {
            var filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Persistence", "teams.json");

            try
            {
                await _teamService.BulkCreateTeamsFromFileAsync(filePath);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message, FilePath = ex.FileName });
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

            return Ok(new { Message = "Teams created successfully." });
        }
    }
}
