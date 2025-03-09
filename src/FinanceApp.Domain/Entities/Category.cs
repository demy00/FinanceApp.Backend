namespace FinanceApp.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public bool IsDeleted { get; private set; }
        public Guid? UserId { get; init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        private Category() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

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

        public void Rename(string newName)
        {
            if (UserId == null)
                throw new InvalidOperationException("Predefined categories cannot be renamed.");

            Name = string.IsNullOrWhiteSpace(newName)
                ? throw new ArgumentException("Category name cannot be empty.", nameof(newName))
                : newName;
        }

        public void UpdateDescription(string? newDescription)
        {
            if (UserId == null)
                throw new InvalidOperationException("Predefined categories cannot be modified.");

            Description = newDescription;
        }

        public void Delete()
        {
            if (UserId == null)
                throw new InvalidOperationException("Predefined categories cannot be deleted.");

            IsDeleted = true;
        }
    }
}
