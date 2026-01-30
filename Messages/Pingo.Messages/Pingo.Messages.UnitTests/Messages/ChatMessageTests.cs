using FluentAssertions;
using Pingo.Messages.Domain;
using Pingo.Messages.UnitTests.Abstractions;

namespace Pingo.Messages.UnitTests.Messages;

public class ChatMessageTests : BaseTest
{
    [Fact]
    public void Create_ShouldReturnMessage()
    {
        var id = Guid.NewGuid();
        var content = Faker.Lorem.Sentence();

        var message = ChatMessage.Create(id, content);

        message.Should().NotBeNull();
    }

    [Fact]
    public void Create_ShouldSetId()
    {
        var id = Guid.NewGuid();
        var content = Faker.Lorem.Sentence();

        var message = ChatMessage.Create(id, content);

        message.Id.Should().Be(id);
    }

    [Fact]
    public void Create_ShouldSetContent()
    {
        var id = Guid.NewGuid();
        var content = Faker.Lorem.Sentence();

        var message = ChatMessage.Create(id, content);

        message.Content.Should().Be(content);
    }

    [Fact]
    public void Create_ShouldSetCreatedAtUtc()
    {
        var id = Guid.NewGuid();
        var content = Faker.Lorem.Sentence();
        var beforeCreation = DateTimeOffset.UtcNow;

        var message = ChatMessage.Create(id, content);

        message.CreatedAtUtc.Should().BeOnOrAfter(beforeCreation);
        message.CreatedAtUtc.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Create_ShouldSetUpdatedAtUtcToNull()
    {
        var id = Guid.NewGuid();
        var content = Faker.Lorem.Sentence();

        var message = ChatMessage.Create(id, content);

        message.UpdatedAtUtc.Should().BeNull();
    }

    [Fact]
    public void UpdateContent_ShouldUpdateContent()
    {
        var message = ChatMessage.Create(Guid.NewGuid(), Faker.Lorem.Sentence());
        var newContent = Faker.Lorem.Paragraph();

        message.UpdateContent(newContent);

        message.Content.Should().Be(newContent);
    }

    [Fact]
    public void UpdateContent_ShouldSetUpdatedAtUtc()
    {
        var message = ChatMessage.Create(Guid.NewGuid(), Faker.Lorem.Sentence());
        var beforeUpdate = DateTimeOffset.UtcNow;

        message.UpdateContent(Faker.Lorem.Paragraph());

        message.UpdatedAtUtc.Should().NotBeNull();
        message.UpdatedAtUtc.Should().BeOnOrAfter(beforeUpdate);
        message.UpdatedAtUtc.Should().BeOnOrBefore(DateTimeOffset.UtcNow);
    }

    [Fact]
    public void UpdateContent_ShouldNotUpdate_WhenContentIsSame()
    {
        var content = Faker.Lorem.Sentence();
        var message = ChatMessage.Create(Guid.NewGuid(), content);

        message.UpdateContent(content);

        message.UpdatedAtUtc.Should().BeNull();
        message.Content.Should().Be(content);
    }

    [Fact]
    public void UpdateContent_ShouldNotChangeCreatedAtUtc()
    {
        var message = ChatMessage.Create(Guid.NewGuid(), Faker.Lorem.Sentence());
        var createdAt = message.CreatedAtUtc;

        message.UpdateContent(Faker.Lorem.Paragraph());

        message.CreatedAtUtc.Should().Be(createdAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Hello World")]
    [InlineData("Привет мир!")]
    public void Create_ShouldAcceptVariousContent(string content)
    {
        var id = Guid.NewGuid();

        var message = ChatMessage.Create(id, content);

        message.Content.Should().Be(content);
    }
}
