using MeditationCountBot.Dto;

namespace MeditationCountBot.Services;

public class CalculateContinuouslyService : ICalculateContinuouslyService
{
    public void CalculateContinuouslyDays(CounterDto counterDto, DateTime dateTime)
    {
        foreach (var participantDto in counterDto.Participants)
        {
            if (participantDto.LastMeditation.Date == dateTime.Date)
            {
                participantDto.ContinuouslyDays += 1;
            }
            else
            {
                participantDto.ContinuouslyDays = 0;
            }
        }
    }
}