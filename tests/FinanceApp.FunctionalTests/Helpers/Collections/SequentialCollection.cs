using FinanceApp.FunctionalTests.Setup;

namespace FinanceApp.FunctionalTests.Helpers.Collections;

[CollectionDefinition("Sequential Tests Collection", DisableParallelization = true)]
public class SequentialCollection : ICollectionFixture<PostgreSqlTestFixture>
{
    // This class is just a marker for the collection.
}
