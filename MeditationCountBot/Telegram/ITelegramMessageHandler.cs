using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MeditationCountBot.Telegram;

public interface ITelegramMessageHandler
{
    Task HandleErrorAsync(Exception exception, HandleErrorSource source);

    Task HandleMessageAsync(Message msg, UpdateType type);
}