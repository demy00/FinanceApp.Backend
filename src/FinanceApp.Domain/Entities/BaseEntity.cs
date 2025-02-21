namespace FinanceApp.Domain.Entities;

public abstract class BaseEntity : IEquatable<BaseEntity>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool Equals(BaseEntity? other)
    {
        if (other is null || GetType() != other.GetType()) return false;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is BaseEntity entity)
        {
            return Equals(entity);
        }
        return false;
    }

    public override int GetHashCode() => Id.GetHashCode();
}
