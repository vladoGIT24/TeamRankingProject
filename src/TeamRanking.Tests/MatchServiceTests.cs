using Match = TeamRanking.Persistence.Entity.Match;

namespace TeamRanking.Tests
{
    public class MatchServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly Mock<IRankingService> _rankingServiceMock;
        private readonly MatchService _matchService;

        public MatchServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _rankingServiceMock = new Mock<IRankingService>();
            _matchService = new MatchService(_fixture.DbContext, _rankingServiceMock.Object);
        }

        private async Task CleanUp()
        {
            _fixture.DbContext.Matches.RemoveRange(_fixture.DbContext.Matches);
            _fixture.DbContext.Teams.RemoveRange(_fixture.DbContext.Teams);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllMatchesAsync_ShouldReturnAllMatches()
        {
            // Arrange
            await CleanUp();
            var teamA = new Team { TeamName = "Team A", IsDeleted = false };
            var teamB = new Team { TeamName = "Team B", IsDeleted = false };
            _fixture.DbContext.Teams.AddRange(teamA, teamB);
            await _fixture.DbContext.SaveChangesAsync();

            var match = new Match { HomeTeamId = teamA.TeamId, AwayTeamId = teamB.TeamId, HomeTeamScore = 1, AwayTeamScore = 2, IsOver = true };
            _fixture.DbContext.Matches.Add(match);
            await _fixture.DbContext.SaveChangesAsync();

            // Act
            var result = await _matchService.GetAllMatchesAsync();

            // Assert
            Assert.Single(result);
            Assert.Contains(result, m => m.HomeTeamId == teamA.TeamId && m.AwayTeamId == teamB.TeamId);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task GetMatchByIdAsync_ShouldReturnMatch_WhenMatchExists()
        {
            // Arrange
            await CleanUp();
            var teamA = new Team { TeamName = "Team A", IsDeleted = false };
            var teamB = new Team { TeamName = "Team B", IsDeleted = false };
            _fixture.DbContext.Teams.AddRange(teamA, teamB);
            await _fixture.DbContext.SaveChangesAsync();

            var match = new Match { HomeTeamId = teamA.TeamId, AwayTeamId = teamB.TeamId, HomeTeamScore = 1, AwayTeamScore = 2, IsOver = true };
            _fixture.DbContext.Matches.Add(match);
            await _fixture.DbContext.SaveChangesAsync();

            // Act
            var result = await _matchService.GetMatchByIdAsync(match.MatchId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(match.MatchId, result.MatchId);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task GetMatchByIdAsync_ShouldReturnNull_WhenMatchDoesNotExist()
        {
            // Arrange
            await CleanUp();

            // Act
            var result = await _matchService.GetMatchByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateMatchAsync_ShouldAddMatch()
        {
            // Arrange
            await CleanUp();
            var teamA = new Team { TeamName = "Team A", IsDeleted = false };
            var teamB = new Team { TeamName = "Team B", IsDeleted = false };
            _fixture.DbContext.Teams.AddRange(teamA, teamB);
            await _fixture.DbContext.SaveChangesAsync();

            var createMatchDto = new CreateMatchDto { HomeTeamId = teamA.TeamId, AwayTeamId = teamB.TeamId, HomeTeamScore = 1, AwayTeamScore = 2, IsOver = true };

            // Act
            var result = await _matchService.CreateMatchAsync(createMatchDto);
            var match = await _fixture.DbContext.Matches.FirstOrDefaultAsync(m => m.MatchId == result.MatchId);

            // Assert
            Assert.NotNull(match);
            Assert.Equal(createMatchDto.HomeTeamId, match.HomeTeamId);
            Assert.Equal(createMatchDto.AwayTeamId, match.AwayTeamId);
            Assert.Equal(createMatchDto.HomeTeamScore, match.HomeTeamScore);
            Assert.Equal(createMatchDto.AwayTeamScore, match.AwayTeamScore);
            _rankingServiceMock.Verify(r => r.UpdateRankingsAsync(It.IsAny<Match>()), Times.Once);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task UpdateMatchAsync_ShouldUpdateMatch_WhenMatchExists()
        {
            // Arrange
            await CleanUp();
            var teamA = new Team { TeamName = "Team A", IsDeleted = false };
            var teamB = new Team { TeamName = "Team B", IsDeleted = false };
            _fixture.DbContext.Teams.AddRange(teamA, teamB);
            await _fixture.DbContext.SaveChangesAsync();

            var match = new Match { HomeTeamId = teamA.TeamId, AwayTeamId = teamB.TeamId, HomeTeamScore = 1, AwayTeamScore = 2, IsOver = false };
            _fixture.DbContext.Matches.Add(match);
            await _fixture.DbContext.SaveChangesAsync();

            var updateMatchDto = new UpdateMatchDto { HomeTeamScore = 3, AwayTeamScore = 2, IsOver = true };

            // Act
            var result = await _matchService.UpdateMatchAsync(match.MatchId, updateMatchDto);
            var updatedMatch = await _fixture.DbContext.Matches.FindAsync(match.MatchId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateMatchDto.HomeTeamScore, updatedMatch.HomeTeamScore);
            Assert.Equal(updateMatchDto.AwayTeamScore, updatedMatch.AwayTeamScore);
            _rankingServiceMock.Verify(r => r.UpdateRankingsAsync(It.IsAny<Match>()), Times.Once);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task UpdateMatchAsync_ShouldReturnNull_WhenMatchDoesNotExist()
        {
            // Arrange
            await CleanUp();
            var updateMatchDto = new UpdateMatchDto { HomeTeamScore = 3, AwayTeamScore = 2, IsOver = true };

            // Act
            var result = await _matchService.UpdateMatchAsync(1, updateMatchDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteMatchAsync_ShouldRemoveMatch()
        {
            // Arrange
            await CleanUp();
            var teamA = new Team { TeamName = "Team A", IsDeleted = false };
            var teamB = new Team { TeamName = "Team B", IsDeleted = false };
            _fixture.DbContext.Teams.AddRange(teamA, teamB);
            await _fixture.DbContext.SaveChangesAsync();

            var match = new Match { HomeTeamId = teamA.TeamId, AwayTeamId = teamB.TeamId, HomeTeamScore = 1, AwayTeamScore = 2, IsOver = true };
            _fixture.DbContext.Matches.Add(match);
            await _fixture.DbContext.SaveChangesAsync();

            // Act
            await _matchService.DeleteMatchAsync(match.MatchId);
            var deletedMatch = await _fixture.DbContext.Matches.FindAsync(match.MatchId);

            // Assert
            Assert.Null(deletedMatch);

            // Clean up
            await CleanUp();
        }
    }
}