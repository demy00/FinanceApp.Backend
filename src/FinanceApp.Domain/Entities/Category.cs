namespace FinanceApp.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string? Name { get; private set; }
        public string? Description { get; private set; }
        public Guid? UserId { get; init; }

        private Category() { }

        public Category(string name, string description, Guid? userId)
        {
            Name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException("Category name is required.", nameof(name))
                : name;
            Description = description;
            UserId = userId;
        }

        // Overloaded constructor for default/seed categories with a specific Id.
        public Category(Guid id, string name, string description, Guid? userId)
            : base(id)
        {
            Name = string.IsNullOrWhiteSpace(name)
                ? throw new ArgumentException("Category name is required.", nameof(name))
                : name;
            Description = description;
            UserId = userId;
        }

    }
}
