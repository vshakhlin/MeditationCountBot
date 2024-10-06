using MeditationCountBot.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MeditationCountBot.Telegram;

public class TelegramBotService : ITelegramBotService
{
    private ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICounterService _counterService;

    public TelegramBotService(
        IServiceProvider serviceProvider, ICounterService counterService)
    {
        _serviceProvider = serviceProvider;
        _counterService = counterService;
    }

    public void Initialize(string key)
    {
        var botClient = new TelegramBotClient(key, 
            new HttpClient()
            {
                Timeout = TimeSpan.FromMinutes(3),
            });

        _botClient = botClient;
        SubscribeUpdates(botClient, new CancellationToken());
    }

    private void SubscribeUpdates(
        TelegramBotClient botClient,
        CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };

        var handler = new TelegramMessageHandler(_serviceProvider, this, _counterService);
        botClient.StartReceiving(
            handler.HandleUpdateAsync,
            handler.HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
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
            // replyMarkup: new ReplyKeyboardMarkup(new List<KeyboardButton>
            // {
            //     KeyboardButton.WithRequestContact(_localizationService.GetTranslation("SharePhone", language)), 
            // }),
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