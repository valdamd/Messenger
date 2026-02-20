using CSharpFunctionalExtensions;
using Identity.Core.Clock;
using Identity.Core.Repositories;
using Identity.Core.Security;
using Identity.Core.Services.Models;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Identity.Core.Services;

public sealed class IdentityService(
    NpgsqlDataSource dataSource,
    IUserRepository userRepository,
    ICredentialsRepository credentialsRepository,
    IRefreshTokenRepository tokenRepository,
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider,
    ITokenProvider tokenProvider,
    ILogger<IdentityService> logger) : IIdentityService
{
    public async Task<Result<Guid, IdentityError>> RegisterAsync(RegisterUserRequest request)
    {
        var now = dateTimeProvider.UtcNow;
        var user = request.ToEntity(now);
        var (hash, salt) = passwordHasher.HashPassword(request.Password);
        var credential = new PasswordCredentials
        {
            UserId = user.Id,
            Email = request.Email,
            PasswordHash = hash,
            Salt = salt,
            CreatedAtUtc = now,
        };

        await using var connection = await dataSource.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await userRepository.CreateUserAsync(user, connection, transaction);
            await credentialsRepository.AddAsync(credential, connection, transaction);

            await transaction.CommitAsync();
            return Result.Success<Guid, IdentityError>(user.Id);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            await TryRollbackAsync(transaction);
            logger.LogInformation(ex, "User registration conflict for email {Email}", request.Email);
            return Result.Failure<Guid, IdentityError>(IdentityErrors.EmailAlreadyExists);
        }
        catch
        {
            await TryRollbackAsync(transaction);
            throw;
        }
    }

    public async Task<Result<AccessTokens, IdentityError>> LoginAsync(LoginUserRequest request)
    {
        var credential = await credentialsRepository.GetByEmailAsync(request.Email);
        if (credential is null)
        {
            return Result.Failure<AccessTokens, IdentityError>(IdentityErrors.UserNotFound);
        }

        if (!passwordHasher.VerifyPassword(request.Password, credential.PasswordHash, credential.Salt))
        {
            return Result.Failure<AccessTokens, IdentityError>(IdentityErrors.InvalidPassword);
        }

        var user = await userRepository.GetUserByIdAsync(credential.UserId);
        if (user is null)
        {
            return Result.Failure<AccessTokens, IdentityError>(IdentityErrors.UserNotFound);
        }

        var tokens = await GenerateTokensAsync(user.Id, credential.Email);
        return Result.Success<AccessTokens, IdentityError>(tokens);
    }

    public async Task<Result<AccessTokens, IdentityError>> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
    {
        var refreshTokenHash = tokenProvider.HashRefreshToken(refreshTokenRequest.RefreshToken);
        var token = await tokenRepository.GetValidRefreshTokenAsync(refreshTokenHash, refreshTokenRequest.RefreshToken);
        if (token is null)
        {
            return Result.Failure<AccessTokens, IdentityError>(IdentityErrors.InvalidRefreshToken);
        }

        var user = await userRepository.GetUserByIdAsync(token.UserId);
        if (user is null)
        {
            return Result.Failure<AccessTokens, IdentityError>(IdentityErrors.InvalidRefreshToken);
        }

        var email = await credentialsRepository.GetEmailByUserIdAsync(token.UserId);
        if (email is null)
        {
            return Result.Failure<AccessTokens, IdentityError>(IdentityErrors.InvalidRefreshToken);
        }

        await using var connection = await dataSource.OpenConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            await tokenRepository.RevokeTokenAsync(token.Id, connection, transaction);
            var refreshedTokens = await GenerateTokensAsync(token.UserId, email, connection, transaction);
            await transaction.CommitAsync();
            return Result.Success<AccessTokens, IdentityError>(refreshedTokens);
        }
        catch
        {
            await TryRollbackAsync(transaction);
            throw;
        }
    }

    public async Task<Result<UserProfile, IdentityError>> GetUserAsync(Guid id)
    {
        var user = await userRepository.GetUserByIdAsync(id);
        if (user is null)
        {
            return Result.Failure<UserProfile, IdentityError>(IdentityErrors.UserNotFound);
        }

        var email = await credentialsRepository.GetEmailByUserIdAsync(id);
        if (email is null)
        {
            return Result.Failure<UserProfile, IdentityError>(IdentityErrors.UserNotFound);
        }

        return Result.Success<UserProfile, IdentityError>(user.ToProfile(email));
    }

    public async Task<UnitResult<IdentityError>> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request)
    {
        var updated = await userRepository.UpdateUserAsync(userId, request.Name, dateTimeProvider.UtcNow);

        return updated
            ? UnitResult.Success<IdentityError>()
            : UnitResult.Failure(IdentityErrors.UserNotFound);
    }

    private async Task<AccessTokens> GenerateTokensAsync(
        Guid userId,
        string email,
        NpgsqlConnection? connection = null,
        NpgsqlTransaction? transaction = null)
    {
        var accessToken = tokenProvider.GenerateAccessToken(userId, email);
        var refreshToken = tokenProvider.GenerateRefreshToken();
        var expiresAt = tokenProvider.GetRefreshTokenExpiration();

        var token = new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Token = null,
            TokenHash = tokenProvider.HashRefreshToken(refreshToken),
            ExpiresAtUtc = expiresAt,
            CreatedAtUtc = dateTimeProvider.UtcNow,
            IsRevoked = false,
        };

        if (connection is not null && transaction is not null)
        {
            await tokenRepository.CreateRefreshTokenAsync(token, connection, transaction);
        }
        else
        {
            await tokenRepository.CreateRefreshTokenAsync(token);
        }

        return new AccessTokens(accessToken, refreshToken);
    }

    private static async Task TryRollbackAsync(NpgsqlTransaction transaction)
    {
        try
        {
            await transaction.RollbackAsync();
        }
        catch
        {
            // Ignore rollback failures to preserve original exception.
        }
    }
}
