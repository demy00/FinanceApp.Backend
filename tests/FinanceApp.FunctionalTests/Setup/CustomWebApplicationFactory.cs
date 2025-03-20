using FinanceApp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceApp.FunctionalTests.Setup;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing ApplicationDbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register the ApplicationDbContext with the test PostgreSQL connection string
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                    ?? "Host=localhost;Port=5432;Database=testdb;Username=postgres;Password=postgres";

                options.UseNpgsql(connectionString);
            });

            // Build the service provider and ensure the database is created
            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        // Clean up the test database before disposal.
        using (var scope = Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
        }
        base.Dispose(disposing);
    }
}
