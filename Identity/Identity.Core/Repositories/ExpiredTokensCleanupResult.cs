namespace Identity.Core.Repositories;

internal readonly record struct ExpiredTokensCleanupResult(int DeletedTokensCount);
