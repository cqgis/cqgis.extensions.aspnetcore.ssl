using Xunit;

namespace ssl.test.IntegrationTest.Base
{ 
    [CollectionDefinition(nameof(ServiceTestCollection))]
    public class ServiceTestCollection : ICollectionFixture<ServiceApiFixture>
    {

    }
}