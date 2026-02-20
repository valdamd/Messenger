using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Identity.Api.DTOs.Users;
using Identity.ComponentTests.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Identity.ComponentTests.Tests;

[Collection(TestCollection.Name)]
public sealed class UsersTests(IdentityWebAppFactory factory) : ComponentTestFixture(factory)
{
    [Fact]
    public async Task GetCurrentUser_ShouldReturnUser_WhenAuthenticated()
    {
        var client = CreateClient();
        var tokens = await RegisterUserAsync(client, "me@test.com", "Test User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var response = await client.GetAsync("/api/users/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal("me@test.com", user.Email);
        Assert.Equal("Test User", user.Name);
        Assert.NotEqual(Guid.Empty, user.Id);
    }

    [Fact]
    public async Task GetCurrentUser_ShouldReturn401_WhenNotAuthenticated()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/api/users/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnUser_WhenRequestingOwnProfile()
    {
        var client = CreateClient();
        var tokens = await RegisterUserAsync(client, "byid@test.com", "ById User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var meResponse = await client.GetAsync("/api/users/me");
        var me = await meResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(me);

        var response = await client.GetAsync($"/api/users/{me.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal(me.Id, user.Id);
        Assert.Equal("byid@test.com", user.Email);
    }

    [Fact]
    public async Task GetUserById_ShouldReturn403_WhenRequestingOtherUsersProfile()
    {
        var client = CreateClient();
        var tokens = await RegisterUserAsync(client, "forbidden@test.com", "Forbidden User");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var otherUserId = Guid.NewGuid();
        var response = await client.GetAsync($"/api/users/{otherUserId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateProfile_ShouldReturn204_WithValidData()
    {
        var client = CreateClient();
        var tokens = await RegisterUserAsync(client, "update@test.com", "Old Name");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var response = await client.PutAsJsonAsync("/api/users/me/profile", new UpdateUserProfileDto
        {
            Name = "New Name",
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var meResponse = await client.GetAsync("/api/users/me");
        var user = await meResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.Equal("New Name", user.Name);
    }

    [Fact]
    public async Task UpdateProfile_ShouldReturn400_WithInvalidData()
    {
        var client = CreateClient();
        var tokens = await RegisterUserAsync(client, "invalid-update@test.com", "Valid Name");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

        var response = await client.PutAsJsonAsync("/api/users/me/profile", new UpdateUserProfileDto
        {
            Name = string.Empty,
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problem);
    }

    [Fact]
    public async Task UpdateProfile_ShouldReturn401_WhenNotAuthenticated()
    {
        var client = CreateClient();

        var response = await client.PutAsJsonAsync("/api/users/me/profile", new UpdateUserProfileDto
        {
            Name = "New Name",
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
