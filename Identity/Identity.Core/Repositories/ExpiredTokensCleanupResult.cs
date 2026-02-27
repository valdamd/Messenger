namespace Identity.Core.Repositories;

public readonly record struct ExpiredTokensCleanupResult(int DeletedTokensCount);
