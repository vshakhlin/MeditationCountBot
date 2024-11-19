using MeditationCountBot.Dto;
using Telegram.Bot.Types;

namespace MeditationCountBot.Services;

public interface ICounterService
{
    Task Initialize();
    Task Reload();
    Task<CounterDto> CountAndSave(string chatId, TimeSpan time, User user, DateTime messageDate);
    Task<CounterDto> ReCountAndSave(string chatId, TimeSpan diffTime, User user);
}