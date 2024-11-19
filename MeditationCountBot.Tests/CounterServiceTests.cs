using MeditationCountBot.Dto;
using MeditationCountBot.Services;
using Moq;
using Telegram.Bot.Types;

namespace MeditationCountBot.Tests;

public class CounterServiceTests
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
                    Today = TimeSpan.Zero,
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

    private Mock<IJsonLoader> GetMockJsonLoader()
    {
        var mockJsonLoader = new Mock<IJsonLoader>();
        mockJsonLoader
            .Setup(_ => _.LoadAllJsons<CounterDto>(It.IsAny<string>()))
            .ReturnsAsync(GetMockDict());
        return mockJsonLoader;
    }

    [Fact]
    public async Task CountAndSaveBaseTest()
    {
        var mockJsonLoader = GetMockJsonLoader();

        var counterService = new CounterService(mockJsonLoader.Object);
        await counterService.Initialize();

        var messageDateTime = DateTime.Parse("2024-07-24T09:38:57Z");
        var counterDto = await counterService.CountAndSave("-1002065988567", TimeSpan.FromMinutes(15),
            new User() { Id = 453949424 }, messageDateTime);

        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.IsAny<CounterDto>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Once);

        Assert.Equal(TimeSpan.FromMinutes(15), counterDto.Today);
        Assert.Equal(2, counterDto.Participants.Count);
        Assert.Equal(TimeSpan.FromMinutes(75), counterDto.Participants[0].Total);
        Assert.Equal(1, counterDto.Participants[0].ContinuouslyDays);
        Assert.Equal(messageDateTime, counterDto.Participants[0].LastMeditation);
        Assert.Equal(TimeSpan.FromMinutes(180), counterDto.Participants[1].Total);
        Assert.Equal(0, counterDto.Participants[1].ContinuouslyDays);
        Assert.Equal(messageDateTime, DateTime.Parse("2024-07-24T09:38:57Z"));
    }
    
    [Fact]
    public async Task CountAndSaveTwiceTest()
    {
        var mockJsonLoader = GetMockJsonLoader();

        var counterService = new CounterService(mockJsonLoader.Object);
        await counterService.Initialize();

        var messageDateTime = DateTime.Parse("2024-07-24T09:38:57Z");
        var counterDto1 = await counterService.CountAndSave("-1002065988567", TimeSpan.FromMinutes(15),
            new User() { Id = 453949424 }, messageDateTime);
        var counterDto2 = await counterService.CountAndSave("-1002065988567", TimeSpan.FromMinutes(20),
            new User() { Id = 1083580632 }, messageDateTime);

        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.IsAny<CounterDto>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Exactly(2));

        Assert.Equal(TimeSpan.FromMinutes(35), counterDto2.Today);
        Assert.Equal(2, counterDto2.Participants.Count);
        Assert.Equal(TimeSpan.FromMinutes(75), counterDto2.Participants[0].Total);
        Assert.Equal(1, counterDto2.Participants[0].ContinuouslyDays);
        Assert.Equal(messageDateTime, counterDto2.Participants[0].LastMeditation);
        Assert.Equal(TimeSpan.FromMinutes(200), counterDto2.Participants[1].Total);
        Assert.Equal(0, counterDto2.Participants[1].ContinuouslyDays);
        Assert.Equal(messageDateTime, counterDto2.Participants[1].LastMeditation);
    }

    [Fact]
    public async Task CountAndSaveNewParticipantTest()
    {
        var mockJsonLoader = GetMockJsonLoader();

        var counterService = new CounterService(mockJsonLoader.Object);
        await counterService.Initialize();

        var messageDateTime = DateTime.Parse("2024-07-24T09:38:57Z");
        var counterDto = await counterService.CountAndSave("-1002065988567", TimeSpan.FromMinutes(15), new User()
        {
            Id = 832931269,
            Username = "marina_yogina",
            FirstName = "Marina",
            LastName = "Kovalenko",
        }, messageDateTime);

        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.IsAny<CounterDto>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Once);

        Assert.Equal(TimeSpan.FromMinutes(15), counterDto.Today);
        Assert.Equal(3, counterDto.Participants.Count);
        Assert.Equal(TimeSpan.FromMinutes(15), counterDto.Participants[2].Total);
        Assert.Equal(832931269, counterDto.Participants[2].Id);
        Assert.Equal("marina_yogina", counterDto.Participants[2].Username);
        Assert.Equal("Marina", counterDto.Participants[2].FirstName);
        Assert.Equal("Kovalenko", counterDto.Participants[2].LastName);
        Assert.Equal(0, counterDto.Participants[2].ContinuouslyDays);
        Assert.Equal(messageDateTime, counterDto.Participants[2].LastMeditation);
    }

    [Fact]
    public async Task CountAndSaveNewChatTest()
    {
        var mockJsonLoader = GetMockJsonLoader();

        var counterService = new CounterService(mockJsonLoader.Object);
        await counterService.Initialize();

        var messageDateTime = DateTime.Parse("2024-07-24T09:38:57Z");
        var counterDto = await counterService.CountAndSave("-455396485", TimeSpan.FromMinutes(15), new User()
        {
            Id = 832931269,
            Username = "marina_yogina",
            FirstName = "Marina",
            LastName = "Kovalenko",
        }, messageDateTime);

        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.IsAny<CounterDto>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Once);

        Assert.Equal("-455396485", counterDto.ChatId);
        Assert.Equal(TimeSpan.Zero, counterDto.Yesterday);
        Assert.Equal(TimeSpan.Zero, counterDto.Best);
        Assert.Equal(TimeSpan.FromMinutes(15), counterDto.Today);
        Assert.Equal(1, counterDto.Participants.Count);
        Assert.Equal(TimeSpan.FromMinutes(15), counterDto.Participants[0].Total);
        Assert.Equal(832931269, counterDto.Participants[0].Id);
        Assert.Equal("marina_yogina", counterDto.Participants[0].Username);
        Assert.Equal("Marina", counterDto.Participants[0].FirstName);
        Assert.Equal("Kovalenko", counterDto.Participants[0].LastName);
        Assert.Equal(0, counterDto.Participants[0].ContinuouslyDays);
        Assert.Equal(messageDateTime, counterDto.Participants[0].LastMeditation);
    }

    [Fact]
    public async Task ReCountAndSaveAddTest()
    {
        var mockJsonLoader = GetMockJsonLoader();

        var counterService = new CounterService(mockJsonLoader.Object);
        await counterService.Initialize();

        var messageDateTime = DateTime.Parse("2024-07-24T09:38:57Z");
        var counterDto = await counterService.CountAndSave("-1002065988567", TimeSpan.FromMinutes(15),
            new User() { Id = 453949424 }, messageDateTime);
        await counterService.ReCountAndSave("-1002065988567", TimeSpan.FromMinutes(2), new User() { Id = 453949424 });

        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.IsAny<CounterDto>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Exactly(2));

        Assert.Equal(TimeSpan.FromMinutes(17), counterDto.Today);
        Assert.Equal(2, counterDto.Participants.Count);
        Assert.Equal(TimeSpan.FromMinutes(60 + 17), counterDto.Participants[0].Total);
        Assert.Equal(1, counterDto.Participants[0].ContinuouslyDays);
        Assert.Equal(messageDateTime, counterDto.Participants[0].LastMeditation);
    }

    [Fact]
    public async Task ReCountAndSaveSubstractTest()
    {
        var mockJsonLoader = GetMockJsonLoader();

        var counterService = new CounterService(mockJsonLoader.Object);
        await counterService.Initialize();

        var messageDateTime = DateTime.Parse("2024-07-24T09:38:57Z");
        var counterDto = await counterService.CountAndSave("-1002065988567", TimeSpan.FromMinutes(15),
            new User() { Id = 453949424 }, messageDateTime);
        await counterService.ReCountAndSave("-1002065988567", TimeSpan.FromMinutes(-15), new User() { Id = 453949424 });

        mockJsonLoader.Verify(
            _ => _.SaveToJsonAsync(It.IsAny<string>(), It.IsAny<CounterDto>(), It.IsAny<string>(), It.IsAny<bool>()),
            Times.Exactly(2));

        Assert.Equal(TimeSpan.FromMinutes(0), counterDto.Today);
        Assert.Equal(2, counterDto.Participants.Count);
        Assert.Equal(TimeSpan.FromMinutes(60), counterDto.Participants[0].Total);
        Assert.Equal(1, counterDto.Participants[0].ContinuouslyDays);
        Assert.Equal(messageDateTime, counterDto.Participants[0].LastMeditation);
    }
}