using MeditationCountBot.Dto;
using MeditationCountBot.Services;

namespace MeditationCountBot.Tests;

public class CalculateContinuouslyServiceTests
{
    [Fact]
    public void CalculateContinuouslyShouldResetContinuouslyDaysTest()
    {
        var counterDto = new CounterDto()
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
        };
        
        var calculateContinuouslyService = new CalculateContinuouslyService();
        calculateContinuouslyService.CalculateContinuouslyDays(counterDto, DateTime.UtcNow);
        
        Assert.Equal(0, counterDto.Participants[0].ContinuouslyDays);
        Assert.Equal(0, counterDto.Participants[1].ContinuouslyDays);
    }
    
    [Fact]
    public void CalculateContinuouslyShouldAddContinuouslyDaysTest()
    {
        var counterDto = new CounterDto()
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
                    ContinuouslyDays = 1,
                    LastMeditation = DateTime.Parse("2024-07-24T23:58:52Z"),
                }
            }
        };
        
        var calculateContinuouslyService = new CalculateContinuouslyService();
        calculateContinuouslyService.CalculateContinuouslyDays(counterDto, DateTime.Parse("2024-07-25T00:00:52Z"));
        
        Assert.Equal(0, counterDto.Participants[0].ContinuouslyDays);
        Assert.Equal(2, counterDto.Participants[1].ContinuouslyDays);
        Assert.Equal(2, counterDto.Participants[1].BestContinuouslyDays);
    }
}