namespace TeamRanking.Services.Models
{
    public class TeamDto
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
    }

    public class CreateUpdateTeamDto
    {
        public string Name { get; set; }
    }
}
    

