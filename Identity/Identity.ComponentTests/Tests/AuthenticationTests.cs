using System.Net;
using System.Net.Http.Json;
using Identity.Api.DTOs.Auth;
using Identity.ComponentTests.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Identity.ComponentTests.Tests;

[Collection(TestCollection.Name)]
public sealed class AuthenticationTests(IdentityWebAppFactory factory) : ComponentTestFixture(factory)
{
    [Fact]
    public async Task Register_ShouldSucceed_WithValidData()
    {
        var client = CreateClient();
        var dto = new RegisterUserDto
        {
            Email = "register@test.com",
            Name = "Test User",
            Password = "SecurePass123!",
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", dto);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<RegisterResponseDto>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.UserId);
    }

    [Fact]
    public async Task Register_ShouldReturnProblemDetails_WhenEmailAlreadyExists()
    {
        var client = CreateClient();
        var dto = new RegisterUserDto
        {
            Email = "duplicate@test.com",
            Name = "Test User",
            Password = "SecurePass123!",
        };

        await client.PostAsJsonAsync("/api/auth/register", dto);

        var response = await client.PostAsJsonAsync("/api/auth/register", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains("email", problem.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_ShouldReturnTokens_WithValidCredentials()
    {
        var client = CreateClient();
        const string email = "login@test.com";
        const string password = "SecurePass123!";

        await client.PostAsJsonAsync("/api/auth/register", new RegisterUserDto
        {
            Email = email,
            Name = "Test User",
            Password = password,
        });

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginUserDto
        {
            Email = email,
            Password = password,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tokens = await response.Content.ReadFromJsonAsync<AccessTokensDto>();
        Assert.NotNull(tokens);
        Assert.NotNull(tokens.AccessToken);
        Assert.NotNull(tokens.RefreshToken);
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WithGenericMessage_WhenUserNotFound()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginUserDto
        {
            Email = "nonexistent@test.com",
            Password = "SecurePass123!",
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("Invalid email or password.", problem.Detail);
        Assert.True(problem.Extensions.TryGetValue("code", out var code));
        Assert.Equal("Identity.InvalidCredentials", code?.ToString());
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WithGenericMessage_WhenPasswordIsWrong()
    {
        var client = CreateClient();
        const string email = "wrongpass@test.com";

        await client.PostAsJsonAsync("/api/auth/register", new RegisterUserDto
        {
            Email = email,
            Name = "Test User",
            Password = "SecurePass123!",
        });

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginUserDto
        {
            Email = email,
            Password = "WrongPassword!",
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal("Invalid email or password.", problem.Detail);
        Assert.True(problem.Extensions.TryGetValue("code", out var code));
        Assert.Equal("Identity.InvalidCredentials", code?.ToString());
    }

    [Fact]
    public async Task Refresh_ShouldReturnNewTokens_WithValidRefreshToken()
    {
        var client = CreateClient();
        const string email = "refresh@test.com";
        const string password = "SecurePass123!";

        await client.PostAsJsonAsync("/api/auth/register", new RegisterUserDto
        {
            Email = email,
            Name = "Test User",
            Password = password,
        });

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginUserDto
        {
            Email = email,
            Password = password,
        });
        var initialTokens = await loginResponse.Content.ReadFromJsonAsync<AccessTokensDto>();
        Assert.NotNull(initialTokens);

        var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh-tokens", new RefreshTokenDto
        {
            RefreshToken = initialTokens.RefreshToken,
        });

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        var newTokens = await refreshResponse.Content.ReadFromJsonAsync<AccessTokensDto>();
        Assert.NotNull(newTokens);
        Assert.NotEqual(initialTokens.AccessToken, newTokens.AccessToken);
        Assert.NotEqual(initialTokens.RefreshToken, newTokens.RefreshToken);
    }

    [Fact]
    public async Task Refresh_ShouldReturnProblemDetails_WhenTokenAlreadyUsed()
    {
        var client = CreateClient();
        const string email = "reuse@test.com";
        const string password = "SecurePass123!";

        await client.PostAsJsonAsync("/api/auth/register", new RegisterUserDto
        {
            Email = email,
            Name = "Test User",
            Password = password,
        });

        var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginUserDto
        {
            Email = email,
            Password = password,
        });
        var tokens = await loginResponse.Content.ReadFromJsonAsync<AccessTokensDto>();
        Assert.NotNull(tokens);

        await client.PostAsJsonAsync("/api/auth/refresh-tokens", new RefreshTokenDto
        {
            RefreshToken = tokens.RefreshToken,
        });

        var secondResponse = await client.PostAsJsonAsync("/api/auth/refresh-tokens", new RefreshTokenDto
        {
            RefreshToken = tokens.RefreshToken,
        });

        Assert.Equal(HttpStatusCode.Unauthorized, secondResponse.StatusCode);
        var problem = await secondResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
    }

    [Fact]
    public async Task Refresh_ShouldReturnProblemDetails_WhenTokenIsInvalid()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/refresh-tokens", new RefreshTokenDto
        {
            RefreshToken = "completely-invalid-token-value",
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
    }
}
