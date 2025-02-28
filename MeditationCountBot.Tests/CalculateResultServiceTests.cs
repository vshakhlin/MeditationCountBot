using MeditationCountBot.Dto;
using MeditationCountBot.Services;
using MeditationCountBot.Telegram;
using Microsoft.Extensions.Logging;
using Moq;

namespace MeditationCountBot.Tests;

public class CalculateResultServiceTests
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
                    Today = TimeSpan.FromMinutes(123),
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
                            ContinuouslyDays = 2,
                            LastMeditation = DateTime.Parse("2024-07-24T09:38:57Z")
                        },
                        new ParticipantDto()
                        {
                            Id = 832931269,
                            LastName = "Kovalenko",
                            FirstName = "Marina",
                            Username = "marina_yogina",
                            Total = TimeSpan.FromMinutes(180),
                            ContinuouslyDays = 2,
                            LastMeditation = DateTime.Parse("2024-07-24T23:38:57Z")
                        }
                    }
                }
            },
            {
                "-455396485", new CounterDto()
                {
                    ChatId = "-455396485",
                    Best = TimeSpan.FromMinutes(100),
                    Yesterday = TimeSpan.FromMinutes(100),
                    Today = TimeSpan.Zero,
                    Settings = new SettingsDto() { TimeZone = TimeSpan.FromHours(3)},
                    Participants = new List<ParticipantDto>()
                }
            },
            {
                "-10020659885678", new CounterDto()
                {
                    ChatId = "-1002065988568",
                    Best = TimeSpan.FromMinutes(100),
                    Yesterday = TimeSpan.FromMinutes(100),
                    Today = TimeSpan.FromMinutes(123),
                    Settings = new SettingsDto() { TimeZone = TimeSpan.FromHours(8)},
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
                    }
                }
            },
            {
                "-10020659885679", new CounterDto()
                {
                    ChatId = "-1002065988569",
                    Best = TimeSpan.FromMinutes(100),
                    Yesterday = TimeSpan.FromMinutes(100),
                    Today = TimeSpan.FromMinutes(123),
                    Settings = new SettingsDto() { TimeZone = TimeSpan.FromHours(-5)},
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
                    }
                }
            }
        };
    }

    private Mock<IJsonLoader> GetMockJsonLoaderForMessageStore()
    {
        var mockJsonLoader = new Mock<IJsonLoader>();
        return mockJsonLoader;
    }

    private Mock<IJsonLoader> GetMockJsonLoader()
    {
        var mockJsonLoader = new Mock<IJsonLoader>();
        mockJsonLoader
            .Setup(_ => _.LoadAllJsons<CounterDto>(It.IsAny<string>()))
            .ReturnsAsync(GetMockDict());
        return mockJsonLoader;
    }


    [Fact]
    public async Task CalculateAndSendBaseTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var mockJsonLoaderForMessagesStore = GetMockJsonLoaderForMessageStore();
        var mockPraiseAndCheerMessage = new Mock<IPraiseAndCheerMessage>();
        mockPraiseAndCheerMessage.Setup(_ => _.GetRandomCheerMessage()).Returns("Не сдавайтесь\\! 🙏");
        
        var messageFormatter = new MessageFormer(new TimeFormatter(), mockPraiseAndCheerMessage.Object, new MeditationMessageProvider());
        var mockTelegramMessageSender = new Mock<ITelegramMessageSender>();
        mockTelegramMessageSender.Setup(_ => _.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendingResult()
            {
                IsSuccess = true,
            });
        var dateTimeService = new Mock<IDateTimeService>();
        dateTimeService.Setup(_ => _.GetDateTimeUtcNow())
            .Returns(DateTime.Parse("2024-07-24T21:00:57Z"));
        dateTimeService.Setup(_ => _.GetDateTimeNow(It.Is<TimeSpan>(ts => ts == TimeSpan.FromHours(3))))
            .Returns(DateTime.Parse("2024-07-25T00:01:57Z"));
        dateTimeService.Setup(_ => _.GetDateTimeNow(It.Is<TimeSpan>(ts => ts == TimeSpan.FromHours(8))))
            .Returns(DateTime.Parse("2024-07-25T05:00:57Z"));
        dateTimeService.Setup(_ => _.GetDateTimeNow(It.Is<TimeSpan>(ts => ts == TimeSpan.FromHours(-5))))
            .Returns(DateTime.Parse("2024-07-24T16:01:57Z"));
        var messagesStore = new MessagesStore(mockJsonLoaderForMessagesStore.Object);
        var counterService = new CounterService(mockJsonLoader.Object, dateTimeService.Object);
        
        var calculateResultService = new CalculateResultService(
            messageFormatter,
            mockJsonLoader.Object,
            mockTelegramMessageSender.Object,
            counterService,
            new CalculateContinuouslyService(),
            messagesStore,
            dateTimeService.Object,
            Mock.Of<ILogger<CalculateResultService>>());

        var result = await calculateResultService.CalculateTotalResultsAndSend();

        // should execute clean on message store
        mockJsonLoaderForMessagesStore.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.Is<List<MessageLog>>(messages => messages.Count == 0), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Once);
        
        // should execute for save new counter
        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.Is<CounterDto>(counter => counter.Yesterday == TimeSpan.FromMinutes(123) && counter.Today == TimeSpan.Zero), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Once);
        
        var expectedMessage = "Общее время медитации 123 \\(2 часа 3 минуты\\)\nНа 197 меньше чем вчера\\. Не сдавайтесь\\! 🙏\n\nМедитируют 3 дня подряд:\n \\- Anna Po \\(@AnnaPot23\\)\n \\- Marina Kovalenko \\(@marina\\_yogina\\)";
        mockTelegramMessageSender.Verify(
            _ => _.SendMessage(
                It.Is<string>(chatId => chatId == "-1002065988567"),
                It.Is<string>(message => message == expectedMessage),
                It.Is<string>(lang => lang == "ru"),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        
        Assert.Single(result);
    }
    
    [Fact]
    public async Task CalculateAndSendHabitTest()
    {
        var mockJsonLoader = GetMockJsonLoader();
        var mockJsonLoaderForMessagesStore = GetMockJsonLoaderForMessageStore();
        var mockPraiseAndCheerMessage = new Mock<IPraiseAndCheerMessage>();
        mockPraiseAndCheerMessage.Setup(_ => _.GetRandomCheerMessage()).Returns("Не сдавайтесь\\! 🙏");
        
        var messageFormatter = new MessageFormer(new TimeFormatter(), mockPraiseAndCheerMessage.Object, new HabitMessageProvider());
        var mockTelegramMessageSender = new Mock<ITelegramMessageSender>();
        mockTelegramMessageSender.Setup(_ => _.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SendingResult()
            {
                IsSuccess = true,
            });
        var dateTimeService = new Mock<IDateTimeService>();
        dateTimeService.Setup(_ => _.GetDateTimeUtcNow())
            .Returns(DateTime.Parse("2024-07-24T21:00:57Z"));
        dateTimeService.Setup(_ => _.GetDateTimeNow(It.Is<TimeSpan>(ts => ts == TimeSpan.FromHours(3))))
            .Returns(DateTime.Parse("2024-07-25T00:01:57Z"));
        dateTimeService.Setup(_ => _.GetDateTimeNow(It.Is<TimeSpan>(ts => ts == TimeSpan.FromHours(8))))
            .Returns(DateTime.Parse("2024-07-25T05:00:57Z"));
        dateTimeService.Setup(_ => _.GetDateTimeNow(It.Is<TimeSpan>(ts => ts == TimeSpan.FromHours(-5))))
            .Returns(DateTime.Parse("2024-07-24T16:01:57Z"));
        var messagesStore = new MessagesStore(mockJsonLoaderForMessagesStore.Object);
        var counterService = new CounterService(mockJsonLoader.Object, dateTimeService.Object);
        
        var calculateResultService = new CalculateResultService(
            messageFormatter,
            mockJsonLoader.Object,
            mockTelegramMessageSender.Object,
            counterService,
            new CalculateContinuouslyService(),
            messagesStore,
            dateTimeService.Object,
            Mock.Of<ILogger<CalculateResultService>>());

        var result = await calculateResultService.CalculateTotalResultsAndSend();

        // should execute clean on message store
        mockJsonLoaderForMessagesStore.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.Is<List<MessageLog>>(messages => messages.Count == 0), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Once);
        
        // should execute for save new counter
        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.Is<CounterDto>(counter => counter.Yesterday == TimeSpan.FromMinutes(123) && counter.Today == TimeSpan.Zero), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Once);
        
        var expectedMessage = "Общее время полезных привычек 123 \\(2 часа 3 минуты\\)\nНа 197 меньше чем вчера\\. Не сдавайтесь\\! 🙏\n\n3 дня подряд:\n \\- Anna Po \\(@AnnaPot23\\)\n \\- Marina Kovalenko \\(@marina\\_yogina\\)";
        mockTelegramMessageSender.Verify(
            _ => _.SendMessage(
                It.Is<string>(chatId => chatId == "-1002065988567"),
                It.Is<string>(message => message == expectedMessage),
                It.Is<string>(lang => lang == "ru"),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        
        Assert.Single(result);
    }
}