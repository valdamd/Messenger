using Dapper;
using Identity.Core.DTOs.Auth;
using Identity.Core.DTOs.Users;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Pingo.Identity;

namespace Identity.Core.Services;

public sealed class IdentityService(
    IConfiguration configuration,
    PasswordService passwordService,
    TokenProvider tokenProvider)
{
    private string ConnectionString => configuration.GetConnectionString("DefaultConnection")!;

    public async Task<Guid?> RegisterAsync(RegisterUserDto dto)
    {
        var user = dto.ToEntity();
        var (hash, salt) = passwordService.Hash(dto.Password);

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await connection.ExecuteAsync(
                "INSERT INTO users (id, name, email, created_at_utc) VALUES (@Id, @Name, @Email, @CreatedAtUtc)",
                user, transaction);

            await connection.ExecuteAsync(
                "INSERT INTO user_credentials (user_id, password_hash, salt) VALUES (@UserId, @PasswordHash, @Salt)",
                new
                {
                    UserId = user.Id, PasswordHash = hash, Salt = salt,
                }, transaction);

            await transaction.CommitAsync();
            return user.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            return null;
        }
    }

    public async Task<AccessTokensDto?> LoginAsync(LoginUserDto dto)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var credentials = await connection.QueryFirstOrDefaultAsync<PasswordCredentials>(
            "SELECT user_id as UserId, password_hash as PasswordHash FROM user_credentials uc JOIN users u ON uc.user_id = u.id WHERE u.email = @Email",
            new
            {
                Email = dto.Email.ToLowerInvariant().Trim(),
            });

        if (credentials == null || !passwordService.Verify(dto.Password, credentials.PasswordHash))
        {
            return null;
        }

        var user = await GetUserByIdAsync(credentials.UserId);
        if (user == null)
        {
            return null;
        }

        var accessToken = tokenProvider.GenerateAccessToken(user);
        var refreshToken = tokenProvider.GenerateRefreshToken();

        await connection.ExecuteAsync(
            "INSERT INTO refresh_tokens (id, user_id, token, expires_at_utc) VALUES (@Id, @UserId, @Token, @ExpiresAtUtc)",
            new
            {
                Id = Guid.NewGuid(), UserId = user.Id, Token = refreshToken, ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            });

        return new AccessTokensDto(accessToken, refreshToken);
    }

    public async Task<AccessTokensDto?> RefreshTokenAsync(string token)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        var refreshToken = await connection.QueryFirstOrDefaultAsync<RefreshToken>(
            "SELECT id, user_id as UserId, token, expires_at_utc as ExpiresAtUtc FROM refresh_tokens WHERE token = @Token",
            new
            {
                Token = token,
            });

        if (refreshToken == null || refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return null;
        }

        var user = await GetUserByIdAsync(refreshToken.UserId);
        if (user == null)
        {
            return null;
        }

        var newAccessToken = tokenProvider.GenerateAccessToken(user);
        var newRefreshToken = tokenProvider.GenerateRefreshToken();

        await connection.ExecuteAsync("DELETE FROM refresh_tokens WHERE id = @Id", new
            {
                refreshToken.Id,
            });
        await connection.ExecuteAsync(
            "INSERT INTO refresh_tokens (id, user_id, token, expires_at_utc) VALUES (@Id, @UserId, @Token, @ExpiresAtUtc)",
            new
            {
                Id = Guid.NewGuid(), UserId = user.Id, Token = newRefreshToken, ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            });

        return new AccessTokensDto(newAccessToken, newRefreshToken);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT id, name, email, created_at_utc as CreatedAtUtc, updated_at_utc as UpdatedAtUtc FROM users WHERE id = @Id",
            new
            {
                Id = id,
            });
    }

    public async Task<bool> UpdateProfileAsync(Guid userId, UpdateUserProfileDto dto)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        var affected = await connection.ExecuteAsync(
            "UPDATE users SET name = @Name, updated_at_utc = @UpdatedAtUtc WHERE id = @Id",
            new
            {
                dto.Name, UpdatedAtUtc = DateTime.UtcNow, Id = userId,
            });
        return affected > 0;
    }
}
