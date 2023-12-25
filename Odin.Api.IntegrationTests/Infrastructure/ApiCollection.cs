using Xunit;

namespace Odin.Api.IntegrationTests.Infrastructure;

[CollectionDefinition("ApiCollection")]
public class ApiCollection() : ICollectionFixture<ApiFactory> { }
