namespace FinanceApp.FunctionalTests.Helpers;

public class UserHelper
{
    public string UniqueSuffix { get; }

    public string UserName { get; }
    public string Email { get; }
    public string Password { get; init; } = "StrongPassword@123";
    public string FullName { get; init; } = "Full Name";

    public UserHelper()
    {
        UniqueSuffix = Guid.NewGuid().ToString("N");
        UserName = $"testUser_{UniqueSuffix}";
        Email = $"test_{UniqueSuffix}@example.com";
    }
}
