using FinanceApp.Application.Abstractions;
using FinanceApp.Application.Services;
using FinanceApp.Infrastructure.Repositories;
using FinanceApp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
            ?? configuration.GetConnectionString("DefaultConnection");

        // PostgreSQL
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBillItemRepository, BillItemRepository>();
        services.AddScoped<IBillRepository, BillRepository>();
        services.AddScoped<IPeriodRepository, PeriodRepository>();

        return services;
    }
}
