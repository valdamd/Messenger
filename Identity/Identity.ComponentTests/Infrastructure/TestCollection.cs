namespace Identity.ComponentTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class TestCollection : ICollectionFixture<IdentityWebAppFactory>
{
    public const string Name = "IdentityTests";
}
