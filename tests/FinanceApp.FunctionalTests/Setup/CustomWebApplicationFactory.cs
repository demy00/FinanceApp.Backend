using FinanceApp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceApp.FunctionalTests.Setup;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var context = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (context != null)
            {
                services.Remove(context);

                var options = services
                    .Where(
                        r => r.ServiceType == typeof(DbContextOptions) ||
                        (r.ServiceType.IsGenericType &&
                        r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)))
                    .ToArray();

                foreach (var option in options)
                {
                    services.Remove(option);
                }
            }

            var optionsConfig = services
                .Where(
                    r => r.ServiceType.IsGenericType &&
                    r.ServiceType.GetGenericTypeDefinition() == typeof(IDbContextOptionsConfiguration<>))
                .ToArray();

            foreach (var option in optionsConfig)
            {
                services.Remove(option);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

                options.UseNpgsql(connectionString);
            });

        });
    }
}
