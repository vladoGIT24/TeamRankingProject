using Match = TeamRanking.Persistence.Entity.Match;

namespace TeamRanking.Tests
{
    public class RankingServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly RankingService _rankingService;

        public RankingServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _rankingService = new RankingService(_fixture.DbContext);
        }

        private async Task CleanUp()
        {
            _fixture.DbContext.Ranking.RemoveRange(_fixture.DbContext.Ranking);
            _fixture.DbContext.Teams.RemoveRange(_fixture.DbContext.Teams);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllRankingsAsync_ShouldReturnAllRankings()
        {
            // Arrange
            await CleanUp();
            var teamA = new Team { TeamName = "Team A", IsDeleted = false };
            var teamB = new Team { TeamName = "Team B", IsDeleted = false };
            _fixture.DbContext.Teams.AddRange(teamA, teamB);
            await _fixture.DbContext.SaveChangesAsync();

            _fixture.DbContext.Ranking.AddRange(
                new Ranking { TeamId = teamA.TeamId, Points = 10, PlayedGames = 5, Victories = 3, Draws = 1, Defeats = 1 },
                new Ranking { TeamId = teamB.TeamId, Points = 8, PlayedGames = 5, Victories = 2, Draws = 2, Defeats = 1 }
            );
            await _fixture.DbContext.SaveChangesAsync();

            // Act
            var result = await _rankingService.GetAllRankingsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.TeamName == "Team A" && r.Points == 10);
            Assert.Contains(result, r => r.TeamName == "Team B" && r.Points == 8);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task GetRankingByTeamIdAsync_ShouldReturnRanking_WhenRankingExists()
        {
            // Arrange
            await CleanUp();
            var team = new Team { TeamName = "Team A", IsDeleted = false };
            _fixture.DbContext.Teams.Add(team);
            await _fixture.DbContext.SaveChangesAsync();

            var ranking = new Ranking { TeamId = team.TeamId, Points = 10, PlayedGames = 5, Victories = 3, Draws = 1, Defeats = 1 };
            _fixture.DbContext.Ranking.Add(ranking);
            await _fixture.DbContext.SaveChangesAsync();

            // Act
            var result = await _rankingService.GetRankingByTeamIdAsync(team.TeamId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Team A", result.TeamName);
            Assert.Equal(10, result.Points);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task GetRankingByTeamIdAsync_ShouldReturnNull_WhenRankingDoesNotExist()
        {
            // Arrange
            await CleanUp();

            // Act
            var result = await _rankingService.GetRankingByTeamIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task InitializeRankingForTeamAsync_ShouldCreateNewRanking()
        {
            // Arrange
            await CleanUp();
            var team = new Team { TeamName = "Team A", IsDeleted = false };
            _fixture.DbContext.Teams.Add(team);
            await _fixture.DbContext.SaveChangesAsync();

            // Act
            await _rankingService.InitializeRankingForTeamAsync(team.TeamId);
            var ranking = await _fixture.DbContext.Ranking.FirstOrDefaultAsync(r => r.TeamId == team.TeamId);

            // Assert
            Assert.NotNull(ranking);
            Assert.Equal(team.TeamId, ranking.TeamId);
            Assert.Equal(0, ranking.Points);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task DeleteRankingByTeamIdAsync_ShouldRemoveRanking()
        {
            // Arrange
            await CleanUp();
            var team = new Team { TeamName = "Team A", IsDeleted = false };
            _fixture.DbContext.Teams.Add(team);
            await _fixture.DbContext.SaveChangesAsync();

            var ranking = new Ranking { TeamId = team.TeamId, Points = 10, PlayedGames = 5, Victories = 3, Draws = 1, Defeats = 1 };
            _fixture.DbContext.Ranking.Add(ranking);
            await _fixture.DbContext.SaveChangesAsync();

            // Act
            await _rankingService.DeleteRankingByTeamIdAsync(team.TeamId);
            var deletedRanking = await _fixture.DbContext.Ranking.FirstOrDefaultAsync(r => r.TeamId == team.TeamId);

            // Assert
            Assert.Null(deletedRanking);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task UpdateRankingsAsync_ShouldUpdateRankingsCorrectly()
        {
            // Arrange
            await CleanUp();
            var teamA = new Team { TeamName = "Team A", IsDeleted = false };
            var teamB = new Team { TeamName = "Team B", IsDeleted = false };
            _fixture.DbContext.Teams.AddRange(teamA, teamB);
            await _fixture.DbContext.SaveChangesAsync();

            var rankingA = new Ranking { TeamId = teamA.TeamId, Points = 10, PlayedGames = 5, Victories = 3, Draws = 1, Defeats = 1 };
            var rankingB = new Ranking { TeamId = teamB.TeamId, Points = 8, PlayedGames = 5, Victories = 2, Draws = 2, Defeats = 1 };
            _fixture.DbContext.Ranking.AddRange(rankingA, rankingB);
            await _fixture.DbContext.SaveChangesAsync();

            var match = new Match
            {
                HomeTeamId = teamA.TeamId,
                AwayTeamId = teamB.TeamId,
                HomeTeamScore = 2,
                AwayTeamScore = 1,
                IsOver = true
            };

            // Act
            await _rankingService.UpdateRankingsAsync(match);
            var updatedRankingA = await _fixture.DbContext.Ranking.FirstOrDefaultAsync(r => r.TeamId == teamA.TeamId);
            var updatedRankingB = await _fixture.DbContext.Ranking.FirstOrDefaultAsync(r => r.TeamId == teamB.TeamId);

            // Assert
            Assert.Equal(13, updatedRankingA.Points); // 10 + 3 points for win
            Assert.Equal(6, updatedRankingA.PlayedGames); // 5 + 1 game played
            Assert.Equal(4, updatedRankingA.Victories); // 3 + 1 victory

            Assert.Equal(8, updatedRankingB.Points); // Points remain the same
            Assert.Equal(6, updatedRankingB.PlayedGames); // 5 + 1 game played
            Assert.Equal(2, updatedRankingB.Defeats); // 1 + 1 defeat

            // Clean up
            await CleanUp();
        }
    }
}