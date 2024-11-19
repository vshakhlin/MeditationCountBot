using MeditationCountBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MeditationCountBot.Telegram;

public class TelegramBotService : ITelegramBotService
{
    private ITelegramBotClient _botClient;
    private string _username;
    private readonly ILogger<TelegramMessageHandler> _logger;
    private readonly ICounterService _counterService;
    private readonly IDateTimeService _dateTimeService;
    private readonly IMessagesStore _messagesStore;

    public TelegramBotService(
        ICounterService counterService,
        ILogger<TelegramMessageHandler> logger,
        IDateTimeService dateTimeService, 
        IMessagesStore messagesStore)
    {
        _counterService = counterService;
        _logger = logger;
        _dateTimeService = dateTimeService;
        _messagesStore = messagesStore;
    }

    public void Initialize(string key, string username)
    {
        _logger.LogInformation("Initialize bot");
        var botClient = new TelegramBotClient(key, 
            new HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(3),
            });

        _username = username;
        _botClient = botClient;
        SubscribeUpdates(botClient, new CancellationToken());
    }

    public string BotUsername => _username;

    private void SubscribeUpdates(
        TelegramBotClient botClient,
        CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };

        var handler = new TelegramMessageHandler(this, _counterService, _logger, _dateTimeService, _messagesStore);
        botClient.StartReceiving(
            handler.HandleUpdateAsync,
            handler.HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
    }

    public async Task<Message> SendInstructionMessageAsync(
        long chatIdLong,
        CancellationToken cancellationToken,
        int? messageId)
    {
        var botClient = GetBotClient();

        return await botClient.SendTextMessageAsync(
            chatId: chatIdLong,
            replyToMessageId: messageId, 
            text: "Привет, я бот \\- который суммирует время медитации участников чата\\!\n\nПросто пишите в этот чат \\+ и свое количество минут медитации, например: \\+15, \\+30, \\+60 и т\\.д\\.\nРаз в сутки в конце дня я буду присылать информацию со статистикой: \n\n\"Общее время медитации 202 \\(3 часа 22 минуты\\)\\. Так держать\"\n\nА также буду выдавать список участников, которые медитируют без перерыва:\n\n\"Медитируют 10 дней подряд:\n \\- Александр \nМедитируют 2 дня подряд:\n \\- Марина\"\nи т\\.д\\. \n\nНадеюсь, что это поможет на вашем пути и будет мотивировать продолжать практику❤️🙏 \n_\\(по вопросам, предложениям или просто сказать спасибо пишите в личку @vshakhlin\\)_",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }
    
    public async Task<Message> SendStartOrHelpMessageAsync(
        long chatIdLong,
        string textKey,
        string language,
        CancellationToken cancellationToken)
    {
        var botClient = GetBotClient();

        return await botClient.SendTextMessageAsync(
            chatId: chatIdLong,
            text: "Привет, этот бот создан для подсчета медитаций в группах, по вопросам и предложениям пишите в личку @vshakhlin",
            parseMode: ParseMode.MarkdownV2,
            disableNotification: true,
            cancellationToken: cancellationToken);
    }

    public async Task<Message> SendTextMessageAsync(
        long chatIdLong,
        string text,
        string language,
        string messageId,
        bool isComplain,
        CancellationToken cancellationToken)
    {
        var botClient = GetBotClient();
        
        return await botClient.SendTextMessageAsync(chatId: chatIdLong,
            text: text,
            parseMode: ParseMode.MarkdownV2,
            disableNotification: true,
            // replyToMessageId: update.Message.MessageId,
            // replyMarkup: isComplain ? new InlineKeyboardMarkup(
            //     InlineKeyboardButton.WithCallbackData(
            //         _localizationService.GetTranslation("Complain", language),
            //         messageId)) : default,
            cancellationToken: cancellationToken);
    }

    private ITelegramBotClient GetBotClient()
    {
        return _botClient;
    }
}