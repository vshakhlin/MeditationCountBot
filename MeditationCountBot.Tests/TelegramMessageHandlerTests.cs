using MeditationCountBot.Dto;
using MeditationCountBot.Services;
using MeditationCountBot.Telegram;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MeditationCountBot.Tests;

public class TelegramMessageHandlerTests
{
    private Dictionary<string, CounterDto> GetMockDict()
    {
        return new Dictionary<string, CounterDto>()
        {
            {
                "-1002065988567", new CounterDto()
                {
                    ChatId = "-1002065988567",
                    Best = TimeSpan.FromMinutes(480),
                    Yesterday = TimeSpan.FromMinutes(320),
                    Today = TimeSpan.FromMinutes(60),
                    Settings = new SettingsDto()
                    {
                        TimeZone = TimeSpan.FromHours(3),
                    },
                    Participants = new List<ParticipantDto>()
                    {
                        new ParticipantDto()
                        {
                            Id = 453949424,
                            LastName = "akbayev",
                            FirstName = "beibit",
                            Username = null,
                            Total = TimeSpan.FromMinutes(60),
                            ContinuouslyDays = 1,
                            LastMeditation = DateTime.Parse("2024-07-23T18:20:52Z")
                        },
                        new ParticipantDto()
                        {
                            Id = 1083580632,
                            LastName = "Po",
                            FirstName = "Anna",
                            Username = "AnnaPot23",
                            Total = TimeSpan.FromMinutes(180),
                            ContinuouslyDays = 0,
                            LastMeditation = DateTime.Parse("2024-07-24T09:38:57Z")
                        }
                    }
                }
            }
        };
    }
    
    private Dictionary<string, List<MessageLog>> GetMockMessagesDict()
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
                        TotalTime = TimeSpan.FromMinutes(60),
                        MessageDate = DateTime.Parse("2024-07-23T18:20:52Z")
                    },
                    new MessageLog()
                    {
                        MessageId = 146,
                        Text = "Всем доброе утро!",
                        Time = TimeSpan.Zero,
                        TotalTime = TimeSpan.FromMinutes(60),
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
            .Setup(_ => _.LoadAllJsons<CounterDto>(It.IsAny<string>()))
            .ReturnsAsync(GetMockDict());
        return mockJsonLoader;
    }
    
    private Mock<IJsonLoader> GetMockJsonLoaderForMessagesStore()
    {
        var mockJsonLoader = new Mock<IJsonLoader>();
        mockJsonLoader
            .Setup(_ => _.LoadAllJsons<List<MessageLog>>(It.IsAny<string>()))
            .ReturnsAsync(GetMockMessagesDict());
        return mockJsonLoader;
    }
    
    [Fact]
    public async Task HandleMessageShouldSendStartMessageTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var mockJsonLoaderForMessagesStore = GetMockJsonLoaderForMessagesStore();
        var mockDateTimeService = new Mock<IDateTimeService>();
        var counterService = new CounterService(mockJsonLoader.Object, mockDateTimeService.Object);
        await counterService.Initialize();
        var messagesStore = new MessagesStore(mockJsonLoaderForMessagesStore.Object);
        await messagesStore.Initialize();
        var mockTelegramBotService = new Mock<ITelegramBotService>();
        mockTelegramBotService.SetupGet(_ => _.BotUsername).Returns("usernamebot");
        
        var telegramMessageHandler = new TelegramMessageHandler(
            mockTelegramBotService.Object,
            counterService,
            Mock.Of<ILogger<TelegramMessageHandler>>(),
            messagesStore);

        await telegramMessageHandler.HandleMessageAsync(
            new Message()
            {
                Text = $"/start",
                From = new User()
                {
                    Id = 453949424,
                    IsBot = false,
                    LastName = "akbayev",
                    FirstName = "beibit",
                    Username = null,
                },
                Date = DateTime.Parse("2024-07-24T09:38:57Z"),
                Chat = new Chat()
                {
                    Id = 115601429,
                    Type = ChatType.Private,
                    Title = "akbayev beibit",
                },
            }, UpdateType.Message);
        
        mockTelegramBotService.Verify(
            _ => _.SendStartOrHelpMessageAsync(It.Is<long>(chatId => chatId == 115601429), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
        
        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.IsAny<CounterDto>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Never);
        
        mockJsonLoaderForMessagesStore.Verify(
            _ => _.SaveToJsonAsync(
                It.IsAny<string>(), 
                It.IsAny<List<MessageLog>>(), 
                It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Never);
    }
    
    [Fact]
    public async Task HandleMessageShouldSendInstructionMessageTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var mockJsonLoaderForMessagesStore = GetMockJsonLoaderForMessagesStore();
        var mockDateTimeService = new Mock<IDateTimeService>();
        
        var counterService = new CounterService(mockJsonLoader.Object, mockDateTimeService.Object);
        await counterService.Initialize();
        var messagesStore = new MessagesStore(mockJsonLoaderForMessagesStore.Object);
        await messagesStore.Initialize();
        var mockTelegramBotService = new Mock<ITelegramBotService>();
        mockTelegramBotService.SetupGet(_ => _.BotUsername).Returns("usernamebot");
        var admin = new ChatMemberAdministrator
        {
            User = new User()
            {
                Id = 453949424,
                IsBot = false,
                LastName = "akbayev",
                FirstName = "beibit",
                Username = null,
            }
        };
        mockTelegramBotService.Setup(_ => _.GetChatAdministratorsAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([admin]);
        
        var telegramMessageHandler = new TelegramMessageHandler(
            mockTelegramBotService.Object,
            counterService,
            Mock.Of<ILogger<TelegramMessageHandler>>(),
            messagesStore);

        await telegramMessageHandler.HandleMessageAsync(new Message()
            {
                Text = "usernamebot /help",
                From = new User()
                {
                    Id = 453949424,
                    IsBot = false,
                    LastName = "akbayev",
                    FirstName = "beibit",
                    Username = null,
                },
                Date = DateTime.Parse("2024-07-24T09:38:57Z"),
                Chat = new Chat()
                {
                    Id = -1002065988567,
                    Type = ChatType.Supergroup,
                    Title = "Ретрит на АЛТАЕ 11-18",
                },
            }, UpdateType.Message);
        
        
        mockTelegramBotService.Verify(
            _ => _.SendInstructionMessageAsync(It.Is<long>(chatId => chatId == -1002065988567), It.IsAny<CancellationToken>(), It.IsAny<int?>()),
            Times.Once);
        
        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.IsAny<CounterDto>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Never);
        
        mockJsonLoaderForMessagesStore.Verify(
            _ => _.SaveToJsonAsync(
                It.IsAny<string>(), 
                It.IsAny<List<MessageLog>>(), 
                It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Never);
    }
    
    [Fact]
    public async Task HandleMessageShouldSaveToMessagesStoreTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var mockJsonLoaderForMessagesStore = GetMockJsonLoaderForMessagesStore();
        var mockDateTimeService = new Mock<IDateTimeService>();
        var counterService = new CounterService(mockJsonLoader.Object, mockDateTimeService.Object);
        await counterService.Initialize();
        var messagesStore = new MessagesStore(mockJsonLoaderForMessagesStore.Object);
        await messagesStore.Initialize();
        var mockTelegramBotService = new Mock<ITelegramBotService>();
        mockTelegramBotService.SetupGet(_ => _.BotUsername).Returns("usernamebot");

        var telegramMessageHandler = new TelegramMessageHandler(
            mockTelegramBotService.Object,
            counterService,
            Mock.Of<ILogger<TelegramMessageHandler>>(),
            messagesStore);

        await telegramMessageHandler.HandleMessageAsync(new Message()
            {
                Text = "Всем доброе утро!",
                From = new User()
                {
                    Id = 453949424,
                    IsBot = false,
                    LastName = "akbayev",
                    FirstName = "beibit",
                    Username = null,
                },
                Date = DateTime.Parse("2024-07-24T09:38:57Z"),
                Chat = new Chat()
                {
                    Id = -1002065988567,
                    Type = ChatType.Supergroup,
                    Title = "Ретрит на АЛТАЕ 11-18",
                },
            }, UpdateType.Message);
        
        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.IsAny<CounterDto>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Never);
        
        mockJsonLoaderForMessagesStore.Verify(
            _ => _.SaveToJsonAsync(
                It.IsAny<string>(), 
                It.Is<List<MessageLog>>(messages => messages.Count == 3 && messages[2].Text == "Всем доброе утро!" && messages[2].Time == TimeSpan.Zero), 
                It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Once);
    }
    
    [Fact]
    public async Task HandleMessageShouldUpdateCounterTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var mockJsonLoaderForMessagesStore = GetMockJsonLoaderForMessagesStore();
        var mockDateTimeService = new Mock<IDateTimeService>();
        var counterService = new CounterService(mockJsonLoader.Object, mockDateTimeService.Object);
        await counterService.Initialize();
        var messagesStore = new MessagesStore(mockJsonLoaderForMessagesStore.Object);
        await messagesStore.Initialize();
        var mockTelegramBotService = new Mock<ITelegramBotService>();
        mockTelegramBotService.SetupGet(_ => _.BotUsername).Returns("usernamebot");

        var telegramMessageHandler = new TelegramMessageHandler(
            mockTelegramBotService.Object,
            counterService,
            Mock.Of<ILogger<TelegramMessageHandler>>(),
            messagesStore);

        await telegramMessageHandler.HandleMessageAsync(new Message()
            {
                Text = "Доброе утро! +20",
                From = new User()
                {
                    Id = 453949424,
                    IsBot = false,
                    LastName = "akbayev",
                    FirstName = "beibit",
                    Username = null,
                },
                Date = DateTime.Parse("2024-07-24T09:38:57Z"),
                Chat = new Chat()
                {
                    Id = -1002065988567,
                    Type = ChatType.Supergroup,
                    Title = "Ретрит на АЛТАЕ 11-18",
                },
            }, UpdateType.Message);
        
        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(
                It.IsAny<string>(), 
                It.Is<CounterDto>(counter => counter.Today == TimeSpan.FromMinutes(80)), 
                It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Once);
        
        mockJsonLoaderForMessagesStore.Verify(
            _ => _.SaveToJsonAsync(
                It.IsAny<string>(), 
                It.Is<List<MessageLog>>(messages => messages.Count == 3 && messages[2].Text == "Доброе утро! +20" && messages[2].Time == TimeSpan.FromMinutes(20)), 
                It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Once);
    }
    
    [Fact]
    public async Task HandleMessageShouldUpdateCounterWhenEditMessageTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var mockJsonLoaderForMessagesStore = GetMockJsonLoaderForMessagesStore();
        var mockDateTimeService = new Mock<IDateTimeService>();
        var counterService = new CounterService(mockJsonLoader.Object, mockDateTimeService.Object);
        await counterService.Initialize();
        var messagesStore = new MessagesStore(mockJsonLoaderForMessagesStore.Object);
        await messagesStore.Initialize();

        var telegramMessageHandler = new TelegramMessageHandler(
            Mock.Of<ITelegramBotService>(),
            counterService,
            Mock.Of<ILogger<TelegramMessageHandler>>(),
            messagesStore);

        await telegramMessageHandler.HandleMessageAsync(new Message()
            {
                Id = 146,
                Text = "Доброе утро! +20",
                From = new User()
                {
                    Id = 453949424,
                    IsBot = false,
                    LastName = "akbayev",
                    FirstName = "beibit",
                    Username = null,
                },
                Date = DateTime.Parse("2024-07-24T09:38:57Z"),
                Chat = new Chat()
                {
                    Id = -1002065988567,
                    Type = ChatType.Supergroup,
                    Title = "Ретрит на АЛТАЕ 11-18",
                },
            }, UpdateType.EditedMessage);
        
        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(
                It.IsAny<string>(), 
                It.Is<CounterDto>(counter => counter.Today == TimeSpan.FromMinutes(80)), 
                It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Once);
        
        mockJsonLoaderForMessagesStore.Verify(
            _ => _.SaveToJsonAsync(
                It.IsAny<string>(), 
                It.Is<List<MessageLog>>(messages => messages.Count == 2 && messages[1].Text == "Доброе утро! +20" && messages[1].Time == TimeSpan.FromMinutes(20)), 
                It.IsAny<string>(),
                It.IsAny<bool>()),
            Times.Once);
    }
}