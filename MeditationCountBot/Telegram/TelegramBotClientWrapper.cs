using Telegram.Bot;
using Telegram.Bot.Types;

namespace MeditationCountBot.Telegram;

public class TelegramBotClientWrapper : ITelegramBotClientWrapper
{
    public async Task<ChatMember[]> GetChatAdministratorsAsync(
        ITelegramBotClient botClient,
        ChatId chatId,
        CancellationToken cancellationToken = default
    ) => await botClient.GetChatAdministratorsAsync(chatId, cancellationToken);
}