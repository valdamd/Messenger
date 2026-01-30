using Bogus;

namespace Pingo.Messages.UnitTests.Abstractions;

public abstract class BaseTest
{
    protected static readonly Faker Faker = new();
}
