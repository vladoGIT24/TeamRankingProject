namespace TeamRanking.Persistence.Entity
{
    public class Ranking
    {
        public int RankingId { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int Points { get; set; }
        public int PlayedGames { get; set; }
        public int Victories { get; set; }
        public int Draws { get; set; }
        public int Defeats { get; set; }
    }
}
