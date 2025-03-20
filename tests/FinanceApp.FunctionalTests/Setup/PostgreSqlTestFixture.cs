using FinanceApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace FinanceApp.FunctionalTests.Setup;

public class PostgreSqlTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    public ApplicationDbContext Context { get; private set; } = null!;
    private readonly ILogger<PostgreSqlTestFixture> _logger;
    private DbContextOptions<ApplicationDbContext>? _options;

    public PostgreSqlTestFixture()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });
        _logger = loggerFactory.CreateLogger<PostgreSqlTestFixture>();

        _logger.LogInformation("Initializing PostgreSQL Testcontainer...");

        var postgresConfiguration = new PostgreSqlConfiguration("testdb", "postgres", "postgres");

        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithDatabase(postgresConfiguration.Database)
            .WithImage("postgres:16-alpine")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("Starting PostgreSQL Testcontainer...");
            await _postgreSqlContainer.StartAsync();
            _logger.LogInformation("Testcontainer started. ConnectionString: {0}", _postgreSqlContainer.GetConnectionString());

            Environment.SetEnvironmentVariable("CONNECTION_STRING", _postgreSqlContainer.GetConnectionString());

            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_postgreSqlContainer.GetConnectionString())
                .Options;

            Context = new ApplicationDbContext(_options);

            await Context.Database.EnsureDeletedAsync();
            await Context.Database.EnsureCreatedAsync();
            _logger.LogInformation("Database schema created.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during {InitializeAsync}");
            throw;
        }
    }

    public async Task ResetDatabaseAsync()
    {
        try
        {
            _logger.LogInformation("Resetting database to a clean state...");
            await Context.Database.EnsureDeletedAsync();
            await Context.Database.EnsureCreatedAsync();
            _logger.LogInformation("Database reset complete.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during {ResetDatabaseAsync}");
            throw;
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            _logger.LogInformation("Stopping and disposing PostgreSQL Testcontainer asynchronously...");
            await _postgreSqlContainer.StopAsync();
            await _postgreSqlContainer.DisposeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PostgreSQL Testcontainer disposal.");
            throw;
        }
    }
}
