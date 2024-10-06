using MeditationCountBot.Dto;

namespace MeditationCountBot.Services;

public interface IMessageFormer
{
    string CreateMessage(CounterDto counterDto);
}