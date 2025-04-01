using FinanceApp.FunctionalTests.Setup;

namespace FinanceApp.FunctionalTests.Helpers.Collections;


[CollectionDefinition("Auth Collection")]
public class AuthCollection : IClassFixture<PostgreSqlTestFixture>
{
    // This class is just a marker for the collection.
}
