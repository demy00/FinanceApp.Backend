using FinanceApp.Application.Abstractions;
using FinanceApp.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<BillItem> BillItems { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<Period> Periods { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                builder.Entity(entityType.ClrType)
                    .Property<Guid>("Id")
                    .HasColumnType("uuid")
                    .ValueGeneratedNever();
            }
        }

        builder.Entity<Period>(entity =>
        {
            entity.Ignore(p => p.TotalSpent);

            entity.HasIndex(p => p.UserId);
        });

        builder.Entity<Bill>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Id)
                  .HasColumnType("uuid")
                  .ValueGeneratedNever();

            entity.Ignore(b => b.TotalPrice);

            entity.HasIndex(b => b.UserId);
        });

        builder.Entity<BillItem>(entity =>
        {
            entity.OwnsOne(bi => bi.Price, m =>
            {
                m.Property(m => m.Amount).HasColumnName("Price_Amount");
                m.Property(m => m.Currency).HasColumnName("Price_Currency");
            });

            entity.OwnsOne(bi => bi.Quantity, q =>
            {
                q.Property(q => q.Value).HasColumnName("Quantity_Value");
            });

            entity.HasIndex(bi => bi.UserId);
        });

        builder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.UserId);
        });

        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);
    }
}
