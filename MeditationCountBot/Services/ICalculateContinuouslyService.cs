using MeditationCountBot.Dto;

namespace MeditationCountBot.Services;

public interface ICalculateContinuouslyService
{
    void CalculateContinuouslyDays(CounterDto counterDto, DateTime dateTime);
}