using Telegram.Bot;
using Telegram.Bot.Types;

namespace MeditationCountBot.Telegram;

public interface ITelegramMessageHandler
{
    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken);

    Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
}