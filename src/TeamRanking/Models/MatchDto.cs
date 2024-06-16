namespace TeamRanking.Models
{
    public class MatchDto
    {
        public int MatchId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public bool IsOver { get; set; }
    }

    public class CreateMatchDto
    {
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public bool IsOver { get; set; }
    }

    public class UpdateMatchDto
    {
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public bool IsOver { get; set; }
    }
}
