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
    private readonly IMessagesStore _messagesStore;
    private readonly IMessageProvider _messageProvider;

    public TelegramBotService(
        ICounterService counterService,
        ILogger<TelegramMessageHandler> logger,
        IMessagesStore messagesStore,
        IMessageProvider messageProvider)
    {
        _counterService = counterService;
        _logger = logger;
        _messagesStore = messagesStore;
        _messageProvider = messageProvider;
    }

    public void Initialize(string key, string username)
    {
        _logger.LogInformation("Initialize bot");
        var options = new TelegramBotClientOptions(key)
        {
            RetryThreshold = 120, 
            RetryCount = 2,
        };
        var botClient = new TelegramBotClient(options);

        using var cts = new CancellationTokenSource();
        var bot = new TelegramBotClient(options, cancellationToken: cts.Token);
        
        var handler = new TelegramMessageHandler(this, _counterService, _logger, _messagesStore);
        bot.OnError += handler.HandleErrorAsync;
        bot.OnMessage += handler.HandleMessageAsync;
        
        _username = username;
        _botClient = botClient;
    }

    public string BotUsername => _username;

    public async Task<Message> SendInstructionMessageAsync(
        long chatIdLong,
        CancellationToken cancellationToken,
        int? messageId)
    {
        var botClient = GetBotClient();

        return await botClient.SendMessage(
            chatId: chatIdLong,
            replyParameters: messageId != null ? new ReplyParameters { MessageId = messageId.Value } : null, 
            text: _messageProvider.InstructionMessage,
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }
    
    public async Task<ChatMember[]> GetChatAdministratorsAsync(
        long chatIdLong,
        CancellationToken cancellationToken)
    {
        var botClient = GetBotClient();

        return await botClient.GetChatAdministrators(
            chatId: chatIdLong,
            cancellationToken: cancellationToken);
    }
    
    public async Task<Message> SendStartOrHelpMessageAsync(
        long chatIdLong,
        string textKey,
        string language,
        CancellationToken cancellationToken)
    {
        var botClient = GetBotClient();

        return await botClient.SendMessage(
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
        
        return await botClient.SendMessage(chatId: chatIdLong,
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