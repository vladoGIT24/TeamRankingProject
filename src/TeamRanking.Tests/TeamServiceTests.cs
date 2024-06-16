namespace TeamRanking.Tests
{
    public class TeamServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fixture;
        private readonly Mock<IRankingService> _rankingServiceMock;
        private readonly TeamService _teamService;

        public TeamServiceTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _rankingServiceMock = new Mock<IRankingService>();
            _teamService = new TeamService(_fixture.DbContext, _rankingServiceMock.Object);
        }

        private async Task CleanUp()
        {
            _fixture.DbContext.Teams.RemoveRange(_fixture.DbContext.Teams);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllTeamsAsync_ShouldReturnAllTeams()
        {
            // Arrange
            await CleanUp();
            _fixture.DbContext.Teams.AddRange(new Team { TeamName = "Team A", IsDeleted = false }, new Team { TeamName = "Team B", IsDeleted = false });
            await _fixture.DbContext.SaveChangesAsync();

            // Act
            var result = await _teamService.GetAllTeamsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Name == "Team A");
            Assert.Contains(result, t => t.Name == "Team B");

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task GetTeamByIdAsync_ShouldReturnTeam_WhenTeamExists()
        {
            // Arrange
            await CleanUp();
            var team = new Team { TeamName = "Team A", IsDeleted = false };
            _fixture.DbContext.Teams.Add(team);
            await _fixture.DbContext.SaveChangesAsync();

            // Act
            var result = await _teamService.GetTeamByIdAsync(team.TeamId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Team A", result.Name);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task GetTeamByIdAsync_ShouldReturnNull_WhenTeamDoesNotExist()
        {
            // Arrange
            await CleanUp();

            // Act
            var result = await _teamService.GetTeamByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateTeamAsync_ShouldAddTeam()
        {
            // Arrange
            await CleanUp();
            var createTeamDto = new CreateUpdateTeamDto { Name = "Team A" };

            // Act
            var result = await _teamService.CreateTeamAsync(createTeamDto);
            var team = await _fixture.DbContext.Teams.FirstOrDefaultAsync(t => t.TeamName == "Team A");

            // Assert
            Assert.NotNull(team);
            Assert.Equal("Team A", team.TeamName);
            _rankingServiceMock.Verify(r => r.InitializeRankingForTeamAsync(team.TeamId), Times.Once);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task CreateTeamAsync_ShouldThrowException_WhenTeamNameAlreadyExists()
        {
            // Arrange
            await CleanUp();
            var existingTeam = new Team { TeamName = "Team A", IsDeleted = false };
            _fixture.DbContext.Teams.Add(existingTeam);
            await _fixture.DbContext.SaveChangesAsync();

            var createTeamDto = new CreateUpdateTeamDto { Name = "Team A" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _teamService.CreateTeamAsync(createTeamDto));
            Assert.Equal("The team name 'Team A' already exists.", exception.Message);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task UpdateTeamAsync_ShouldUpdateTeam_WhenTeamExists()
        {
            // Arrange
            await CleanUp();
            var team = new Team { TeamName = "Team A", IsDeleted = false };
            _fixture.DbContext.Teams.Add(team);
            await _fixture.DbContext.SaveChangesAsync();

            var updateTeamDto = new CreateUpdateTeamDto { Name = "Updated Team A" };

            // Act
            var result = await _teamService.UpdateTeamAsync(team.TeamId, updateTeamDto);
            var updatedTeam = await _fixture.DbContext.Teams.FindAsync(team.TeamId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Team A", updatedTeam.TeamName);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task UpdateTeamAsync_ShouldThrowException_WhenTeamNameAlreadyExists()
        {
            // Arrange
            await CleanUp();
            var existingTeam = new Team { TeamName = "Team A", IsDeleted = false };
            var teamToUpdate = new Team { TeamName = "Team B", IsDeleted = false };
            _fixture.DbContext.Teams.AddRange(existingTeam, teamToUpdate);
            await _fixture.DbContext.SaveChangesAsync();

            var updateTeamDto = new CreateUpdateTeamDto { Name = "Team A" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _teamService.UpdateTeamAsync(teamToUpdate.TeamId, updateTeamDto));
            Assert.Equal("The team name 'Team A' already exists.", exception.Message);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task UpdateTeamAsync_ShouldReturnNull_WhenTeamDoesNotExist()
        {
            // Arrange
            await CleanUp();
            var updateTeamDto = new CreateUpdateTeamDto { Name = "Updated Team A" };

            // Act
            var result = await _teamService.UpdateTeamAsync(1, updateTeamDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteTeamAsync_ShouldSoftDeleteTeamAndDeleteRanking()
        {
            // Arrange
            await CleanUp();
            var team = new Team { TeamName = "Team A", IsDeleted = false };
            _fixture.DbContext.Teams.Add(team);
            await _fixture.DbContext.SaveChangesAsync();
            var teamId = team.TeamId;

            // Act
            await _teamService.DeleteTeamAsync(teamId);
            var deletedTeam = await _fixture.DbContext.Teams.FindAsync(teamId);

            // Assert
            Assert.True(deletedTeam.IsDeleted);
            _rankingServiceMock.Verify(r => r.DeleteRankingByTeamIdAsync(teamId), Times.Once);

            // Clean up
            await CleanUp();
        }

        [Fact]
        public async Task BulkCreateTeamsFromFileAsync_ShouldAddNewTeams_WhenTeamsDoNotExist()
        {
            // Arrange
            await CleanUp();
            var filePath = "test_teams.json";
            var jsonData = "[{\"Name\": \"Team A\"}, {\"Name\": \"Team B\"}]";
            await File.WriteAllTextAsync(filePath, jsonData);

            // Act
            await _teamService.BulkCreateTeamsFromFileAsync(filePath);
            var teams = await _fixture.DbContext.Teams.ToListAsync();

            // Assert
            Assert.Equal(2, teams.Count);
            Assert.Contains(teams, t => t.TeamName == "Team A");
            Assert.Contains(teams, t => t.TeamName == "Team B");

            // Clean up
            File.Delete(filePath);
            await CleanUp();
        }
    }
}