namespace FinanceApp.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Guid? UserId { get; init; }

#pragma warning disable CS8618 // Required for EF Core
        private Category() { }
#pragma warning restore CS8618 

        public Category(string name, string description, Guid? userId)
            : base()
        {
            ValidateName(name);
            Name = name;
            Description = description ?? string.Empty;
            UserId = userId;
        }

        // Constructor for seed categories with a specified Id.
        public Category(Guid id, string name, string description, Guid? userId)
            : base(id)
        {
            ValidateName(name);
            Name = name;
            Description = description ?? string.Empty;
            UserId = userId;
        }

        public void Update(string name, string description)
        {
            Rename(name);
            UpdateDescription(description);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Rename(string newName)
        {
            ValidateName(newName);
            ValidatePredefined();
            Name = newName;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDescription(string newDescription)
        {
            ValidatePredefined();
            Description = newDescription ?? string.Empty;
            UpdatedAt = DateTime.UtcNow;
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required.", nameof(name));
        }

        private void ValidatePredefined()
        {
            if (UserId == null)
                throw new InvalidOperationException("Predefined categories cannot be modified.");
        }
    }
}
