namespace TeamRanking.Persistence.Entity
{
    public class Team
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public ICollection<Match>? HomeMatches { get; set; }
        public ICollection<Match>? AwayMatches { get; set; }
        public Ranking? Ranking { get; set; }
        public bool IsDeleted { get; set; }
    }
}
