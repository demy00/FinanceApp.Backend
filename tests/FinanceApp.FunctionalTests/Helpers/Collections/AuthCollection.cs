using FinanceApp.FunctionalTests.Setup;

namespace FinanceApp.FunctionalTests.Helpers.Collections;

[CollectionDefinition("Auth Tests Collection")]
public class AuthCollection : ICollectionFixture<PostgreSqlTestFixture>
{
    // This class is just a marker for the collection.
}
