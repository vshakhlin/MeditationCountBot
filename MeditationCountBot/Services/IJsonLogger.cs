using MeditationCountBot.Dto;

namespace MeditationCountBot.Services;

public interface IJsonLogger
{
    Task Log(string chatId, JsonLog jsonLog);
}