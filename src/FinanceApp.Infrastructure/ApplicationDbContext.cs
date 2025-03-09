using FinanceApp.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<BillItem> BillItems { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<Period> Periods { get; set; }
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

        builder.Entity<Bill>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Id)
                  .HasColumnType("uuid")
                  .ValueGeneratedNever();
        });

        builder.Entity<BillItem>()
            .OwnsOne(b => b.Price, m =>
            {
                m.Property(p => p.Amount).HasColumnName("Price_Amount");
                m.Property(p => p.Currency).HasColumnName("Price_Currency");
            });

        builder.Entity<BillItem>()
            .OwnsOne(b => b.Quantity, q =>
            {
                q.Property(q => q.Value).HasColumnName("Quantity_Value");
            });

        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);
    }
}
