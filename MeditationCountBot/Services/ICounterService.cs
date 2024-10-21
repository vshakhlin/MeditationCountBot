using Telegram.Bot.Types;

namespace MeditationCountBot.Services;

public interface ICounterService
{
    Task Initialize();
    Task Reload();
    Task CountAndSave(string chatId, string text, User user, DateTime messageDate);
}