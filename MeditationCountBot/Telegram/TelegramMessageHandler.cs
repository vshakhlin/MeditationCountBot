using MeditationCountBot.Services;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MeditationCountBot.Telegram;

public class TelegramMessageHandler : ITelegramMessageHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITelegramBotService _telegramBotService;
    private readonly ICounterService _counterService;

    public TelegramMessageHandler(IServiceProvider serviceProvider, ITelegramBotService telegramBotService, ICounterService counterService)
    {
        _telegramBotService = telegramBotService;
        _counterService = counterService;
        _serviceProvider = serviceProvider;
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        // Некоторые действия
        Console.WriteLine(JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }
        
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Некоторые действия
        Console.WriteLine(JsonConvert.SerializeObject(update));
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
                    Console.WriteLine(message.From.Username);
                    Console.WriteLine(message.Text);
                    Console.WriteLine(message.Caption);
                    await _counterService.CountAndSave(
                        message.Chat.Id.ToString(),
                        message.Text,
                        message.Caption,
                        message.From,
                        message.Date.AddHours(3));
                }
            }
        }
    }
}