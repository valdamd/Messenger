namespace Pingo.Messages.ComponentTests.Abstractions;

[CollectionDefinition(nameof(IntegrationTestCollection))]
public sealed class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>;
