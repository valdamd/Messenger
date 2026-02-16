using FluentAssertions;
using NSubstitute;
using Pingo.Messages.Application;
using Pingo.Messages.Domain;
using Pingo.Messages.UnitTests.Abstractions;

namespace Pingo.Messages.UnitTests.Services;

public sealed class MessageServiceTests : BaseTest
{
    private readonly IChatMessageRepository _messageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly MessageService _sut;

    public MessageServiceTests()
    {
        _messageRepository = Substitute.For<IChatMessageRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _sut = new MessageService(_messageRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateOrUpdateAsync_ShouldCreateNewMessage_WhenMessageDoesNotExist()
    {
        var messageId = Guid.NewGuid();
        var content = Faker.Lorem.Sentence();
        _messageRepository.GetAsync(messageId, Arg.Any<CancellationToken>())
            .Returns((ChatMessage?)null);

        await _sut.CreateOrUpdateAsync(messageId, content);

        _messageRepository.Received(1).Insert(Arg.Is<ChatMessage>(m =>
            m.Id == messageId && m.Content == content));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateOrUpdateAsync_ShouldUpdateExistingMessage_WhenMessageExists()
    {
        var messageId = Guid.NewGuid();
        var existingMessage = ChatMessage.Create(messageId, Faker.Lorem.Sentence());
        var newContent = Faker.Lorem.Paragraph();
        _messageRepository.GetAsync(messageId, Arg.Any<CancellationToken>())
            .Returns(existingMessage);

        await _sut.CreateOrUpdateAsync(messageId, newContent);

        _messageRepository.Received(1).Update(existingMessage);
        existingMessage.Content.Should().Be(newContent);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateOrUpdateAsync_ShouldNotCallInsert_WhenMessageExists()
    {
        var messageId = Guid.NewGuid();
        var existingMessage = ChatMessage.Create(messageId, Faker.Lorem.Sentence());
        _messageRepository.GetAsync(messageId, Arg.Any<CancellationToken>())
            .Returns(existingMessage);

        await _sut.CreateOrUpdateAsync(messageId, Faker.Lorem.Paragraph());

        _messageRepository.DidNotReceive().Insert(Arg.Any<ChatMessage>());
    }

    [Fact]
    public async Task CreateOrUpdateAsync_ShouldNotCallUpdate_WhenMessageDoesNotExist()
    {
        var messageId = Guid.NewGuid();
        _messageRepository.GetAsync(messageId, Arg.Any<CancellationToken>())
            .Returns((ChatMessage?)null);

        await _sut.CreateOrUpdateAsync(messageId, Faker.Lorem.Sentence());

        _messageRepository.DidNotReceive().Update(Arg.Any<ChatMessage>());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnMessageResponse_WhenMessageExists()
    {
        var messageId = Guid.NewGuid();
        var content = Faker.Lorem.Sentence();
        var message = ChatMessage.Create(messageId, content);
        _messageRepository.GetAsync(messageId, Arg.Any<CancellationToken>())
            .Returns(message);

        var result = await _sut.GetByIdAsync(messageId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(messageId);
        result.Content.Should().Be(content);
        result.CreatedAtUtc.Should().Be(message.CreatedAtUtc);
        result.UpdatedAtUtc.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenMessageDoesNotExist()
    {
        var messageId = Guid.NewGuid();
        _messageRepository.GetAsync(messageId, Arg.Any<CancellationToken>())
            .Returns((ChatMessage?)null);

        var result = await _sut.GetByIdAsync(messageId);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllMessages()
    {
        var messages = new List<ChatMessage>
        {
            ChatMessage.Create(Guid.NewGuid(), Faker.Lorem.Sentence()),
            ChatMessage.Create(Guid.NewGuid(), Faker.Lorem.Sentence()),
            ChatMessage.Create(Guid.NewGuid(), Faker.Lorem.Sentence()),
        };
        _messageRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(messages);

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(3);
        result.Select(r => r.Id).Should().BeEquivalentTo(messages.Select(m => m.Id));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoMessagesExist()
    {
        _messageRepository.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<ChatMessage>());

        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUpdatedAtUtc_WhenMessageWasUpdated()
    {
        var messageId = Guid.NewGuid();
        var message = ChatMessage.Create(messageId, Faker.Lorem.Sentence());
        message.UpdateContent(Faker.Lorem.Paragraph());
        _messageRepository.GetAsync(messageId, Arg.Any<CancellationToken>())
            .Returns(message);

        var result = await _sut.GetByIdAsync(messageId);

        result.Should().NotBeNull();
        result!.UpdatedAtUtc.Should().NotBeNull();
    }
}
