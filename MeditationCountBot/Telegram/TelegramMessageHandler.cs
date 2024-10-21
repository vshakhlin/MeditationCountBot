using MeditationCountBot.Services;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MeditationCountBot.Telegram;

public class TelegramMessageHandler : ITelegramMessageHandler
{
    private readonly ILogger<TelegramMessageHandler> _logger;
    private readonly ITelegramBotService _telegramBotService;
    private readonly ICounterService _counterService;

    public TelegramMessageHandler(ITelegramBotService telegramBotService, ICounterService counterService, ILogger<TelegramMessageHandler> logger)
    {
        _telegramBotService = telegramBotService;
        _counterService = counterService;
        _logger = logger;
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }
        
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation(JsonConvert.SerializeObject(update));
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;
            var language = message?.From?.LanguageCode ?? "en";
            var chatId = message?.Chat.Id;
            if (chatId != null)
            {
                if (!string.IsNullOrEmpty(message.Text) && (message.Text.ToLower() == "/help" || message.Text.ToLower() == "/start") )
                {
                    await _telegramBotService.SendStartOrHelpMessageAsync(
                        chatId.Value,
                        "Help",
                        language,
                        cancellationToken: cancellationToken);
                
                    return;
                }
                
                if (message.From != null && (!string.IsNullOrEmpty(message.Text) || !string.IsNullOrEmpty(message.Caption)))
                {
                    var text = message.Text;
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        text = message.Caption;
                    }

                    Console.WriteLine(message.From.Username);
                    Console.WriteLine(message.Text);
                    Console.WriteLine(message.Caption);
                    await _counterService.CountAndSave(
                        message.Chat.Id.ToString(),
                        text,
                        message.From,
                        message.Date.AddHours(3));
                }
            }
        } 
        else if (update.Type == UpdateType.EditedMessage)
        {
            // TODO: implement
        }
    }
}