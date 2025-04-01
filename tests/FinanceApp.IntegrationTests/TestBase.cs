using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Infrastructure.Tests;

public abstract class TestBase : IDisposable
{
    protected ApplicationDbContext Context { get; }

    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        Context = new ApplicationDbContext(options);

        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
