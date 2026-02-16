namespace Identity.Core.Clock;

public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
