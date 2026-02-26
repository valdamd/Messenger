namespace Identity.Core.Database;

internal static class DatabaseMigrations
{
    public const string CreateUsersTable = """
        CREATE TABLE IF NOT EXISTS users (
            id UUID PRIMARY KEY,
            name VARCHAR(100) NOT NULL,
            created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
            updated_at_utc TIMESTAMPTZ
        );
        """;

    public const string CreateUserCredentialsTable = """
        CREATE TABLE IF NOT EXISTS user_credentials (
            user_id UUID PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
            email VARCHAR(320) NOT NULL UNIQUE,
            password_hash TEXT NOT NULL,
            salt TEXT NOT NULL,
            created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW()
        );
        CREATE UNIQUE INDEX IF NOT EXISTS idx_user_credentials_email ON user_credentials(LOWER(email));
        """;

    public const string CreateRefreshTokensTable = """
        CREATE TABLE IF NOT EXISTS refresh_tokens (
            id UUID PRIMARY KEY,
            user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
            token TEXT UNIQUE,
            token_hash TEXT,
            expires_at_utc TIMESTAMPTZ NOT NULL,
            created_at_utc TIMESTAMPTZ NOT NULL DEFAULT NOW(),
            is_revoked BOOLEAN NOT NULL DEFAULT FALSE
        );
        CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user_id ON refresh_tokens(user_id);
        CREATE INDEX IF NOT EXISTS idx_refresh_tokens_token ON refresh_tokens(token) WHERE NOT is_revoked;
        CREATE INDEX IF NOT EXISTS idx_refresh_tokens_expires_at ON refresh_tokens(expires_at_utc);
        """;

    public const string AddRefreshTokenHashSupport = """
        ALTER TABLE refresh_tokens ADD COLUMN IF NOT EXISTS token_hash TEXT;
        ALTER TABLE refresh_tokens ALTER COLUMN token DROP NOT NULL;
        CREATE UNIQUE INDEX IF NOT EXISTS idx_refresh_tokens_token_hash ON refresh_tokens(token_hash) WHERE token_hash IS NOT NULL;
        """;

    public static IEnumerable<string> GetAllMigrations()
    {
        yield return CreateUsersTable;
        yield return CreateUserCredentialsTable;
        yield return CreateRefreshTokensTable;
        yield return AddRefreshTokenHashSupport;
    }
}
