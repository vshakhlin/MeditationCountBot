using MeditationCountBot.Dto;

namespace MeditationCountBot.Telegram;

public class TelegramMessageSender : ITelegramMessageSender
{
    private readonly ITelegramBotService _telegramBotService;

    public TelegramMessageSender(ITelegramBotService telegramBotService)
    {
        _telegramBotService = telegramBotService;
    }

    public async Task<SendingResult> SendMessage(
        string chatId,
        string text,
        string language,
        string messageId,
        CancellationToken cancellationToken)
    {
        var sendingResult = new SendingResult();
        if (long.TryParse(chatId, out var chatIdLong))
        {
            try
            {
                var message = await _telegramBotService.SendTextMessageAsync(
                    chatIdLong,
                    text,
                    language,
                    messageId,
                    true,
                    cancellationToken);
                sendingResult.IsSuccess = true;
            }
            catch (Exception e)
            {
                sendingResult.IsSuccess = false;
                sendingResult.ErrorMessage = e.Message;
            }
        }
        else
        {
            sendingResult.ErrorMessage = $"Failed parse telegram chatId {chatId}";
        }

        return sendingResult;
    }
}