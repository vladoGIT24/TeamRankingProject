namespace TeamRanking.Services.Models
{
    public class RankingDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int Points { get; set; }
        public int PlayedGames { get; set; }
        public int Victories { get; set; }
        public int Draws { get; set; }
        public int Defeats { get; set; }
    }
}
