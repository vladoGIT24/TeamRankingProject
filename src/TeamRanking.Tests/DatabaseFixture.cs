public class DatabaseFixture : IDisposable
{
    public ApplicationDbContext DbContext { get; private set; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        DbContext = new ApplicationDbContext(options);
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}