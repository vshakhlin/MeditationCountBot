using MeditationCountBot.Dto;
using MeditationCountBot.Services;
using Moq;

namespace MeditationCountBot.Tests;

public class MessagesStoreTests
{
    private Dictionary<string, List<MessageLog>> GetMockDict()
    {
        return new Dictionary<string, List<MessageLog>>()
        {
            {
                "-1002065988567", new List<MessageLog>()
                {
                    new MessageLog()
                    {
                        MessageId = 145,
                        Text = "+20",
                        Time = TimeSpan.FromMinutes(20),
                        TotalTime = TimeSpan.FromMinutes(20),
                        MessageDate = DateTime.Parse("2024-07-23T18:20:52Z")
                    },
                    new MessageLog()
                    {
                        MessageId = 146,
                        Text = "Just a text",
                        Time = TimeSpan.Zero,
                        TotalTime = TimeSpan.FromMinutes(20),
                        MessageDate = DateTime.Parse("2024-07-23T18:22:52Z")
                    }
                }
            }
        };
    }

    private Mock<IJsonLoader> GetMockJsonLoader()
    {
        var mockJsonLoader = new Mock<IJsonLoader>();
        mockJsonLoader
            .Setup(_ => _.LoadAllJsons<List<MessageLog>>(It.IsAny<string>()))
            .ReturnsAsync(GetMockDict());
        return mockJsonLoader;
    }

    [Fact]
    public async Task ClearTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var messagesStore = new MessagesStore(mockJsonLoader.Object);

        await messagesStore.Initialize();

        await messagesStore.Clear("-1002065988567");
        
        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.Is<List<MessageLog>>(messages => messages.Count == 0), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Once);
    }
    
    [Fact]
    public async Task LoadShouldFoundMessageTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var messagesStore = new MessagesStore(mockJsonLoader.Object);

        await messagesStore.Initialize();

        var message = messagesStore.Load("-1002065988567", 145);
        
        Assert.NotNull(message);
    }
    
    [Fact]
    public async Task LoadNotFoundMessageTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var messagesStore = new MessagesStore(mockJsonLoader.Object);

        await messagesStore.Initialize();

        var message = messagesStore.Load("-1002065988567", 25);
        
        Assert.Null(message);
    }
    
    [Fact]
    public async Task SaveTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var messagesStore = new MessagesStore(mockJsonLoader.Object);

        await messagesStore.Initialize();
        await messagesStore.Save("-1002065988567", new MessageLog()
        {
            MessageId = 147,
            Text = "+39",
            Time = TimeSpan.FromMinutes(39),
            TotalTime = TimeSpan.FromMinutes(20),
            MessageDate = DateTime.Parse("2024-07-23T18:22:52Z")
        });

        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.Is<List<MessageLog>>(messages => messages.Count == 3), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Once);
    }
    
    [Fact]
    public async Task SaveForNewChatTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var messagesStore = new MessagesStore(mockJsonLoader.Object);

        await messagesStore.Initialize();
        await messagesStore.Save("-455396485", new MessageLog()
        {
            MessageId = 20,
            Text = "+30",
            Time = TimeSpan.FromMinutes(30),
            TotalTime = TimeSpan.FromMinutes(20),
            MessageDate = DateTime.Parse("2024-07-23T18:22:52Z")
        });

        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.Is<List<MessageLog>>(messages => messages.Count == 1), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Once);
    }
    
    [Fact]
    public async Task ReSaveTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var messagesStore = new MessagesStore(mockJsonLoader.Object);

        await messagesStore.Initialize();
        await messagesStore.ReSave("-1002065988567", new MessageLog()
        {
            MessageId = 146,
            Text = "+39",
            Time = TimeSpan.FromMinutes(39),
            TotalTime = TimeSpan.FromMinutes(20),
            MessageDate = DateTime.Parse("2024-07-23T18:22:52Z")
        });

        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(
                It.IsAny<string>(), 
                It.Is<List<MessageLog>>(messages => messages.Count == 2 && messages[1].Text == "+39" && messages[1].Time == TimeSpan.FromMinutes(39)), 
                It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Once);
    }
}