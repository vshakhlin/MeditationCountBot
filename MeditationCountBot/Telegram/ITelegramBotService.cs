using Telegram.Bot.Types;

namespace MeditationCountBot.Telegram;

public interface ITelegramBotService
{
    void Initialize(string key, string username);

     Task<Message> SendInstructionMessageAsync(
        long chatIdLong,
        CancellationToken cancellationToken,
        int? messageId);
     
    Task<Message> SendStartOrHelpMessageAsync(
        long chatIdLong,
        string textKey,
        string language,
        CancellationToken cancellationToken);

    Task<Message> SendTextMessageAsync(
        long chatIdLong,
        string text,
        string language,
        string messageId,
        bool isComplain,
        CancellationToken cancellationToken);

    string BotUsername { get; }
}