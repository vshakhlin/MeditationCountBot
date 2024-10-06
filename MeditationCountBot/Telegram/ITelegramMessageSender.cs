using MeditationCountBot.Dto;

namespace MeditationCountBot.Telegram;

public interface ITelegramMessageSender
{
    Task<SendingResult> SendMessage(
        string chatId,
        string text,
        string language,
        string messageId,
        CancellationToken cancellationToken);
}