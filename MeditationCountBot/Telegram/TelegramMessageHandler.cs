using MeditationCountBot.Dto;
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
    private readonly IDateTimeService _dateTimeService;
    private readonly IMessagesStore _messagesStore;

    public TelegramMessageHandler(
        ITelegramBotService telegramBotService,
        ICounterService counterService,
        ILogger<TelegramMessageHandler> logger,
        IDateTimeService dateTimeService,
        IMessagesStore messagesStore)
    {
        _telegramBotService = telegramBotService;
        _counterService = counterService;
        _logger = logger;
        _dateTimeService = dateTimeService;
        _messagesStore = messagesStore;
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
                if (message.Chat.Type == ChatType.Private && !string.IsNullOrEmpty(message.Text) && (message.Text.ToLower() == "/help" || message.Text.ToLower() == "/start"))
                {
                    await _telegramBotService.SendStartOrHelpMessageAsync(
                        chatId.Value,
                        "Help",
                        language,
                        cancellationToken: cancellationToken);
                
                    return;
                }

                if (!string.IsNullOrEmpty(message.Text) && message.Text.ToLower().Contains(_telegramBotService.BotUsername) && message.Text.ToLower().Contains("/help"))
                {
                    var admins = await botClient.GetChatAdministratorsAsync(message.Chat.Id, cancellationToken);
                    if (admins.Any(cm => cm.User.Id == message.From.Id))
                    {
                        await _telegramBotService.SendInstructionMessageAsync(
                            chatId.Value,
                            cancellationToken: cancellationToken,
                            message.MessageId);

                    }

                    return;
                }

                // if (message.NewChatMembers != null && message.NewChatMembers.Any(cm => cm.Username.ToLower() == BotUsername))
                // {
                //     await _telegramBotService.SendHelloMessageAsync(
                //         chatId.Value,
                //         cancellationToken: cancellationToken,
                //         message.MessageId);
                //
                //     return;
                // }
                
                if (message.From != null && !message.From.IsBot && (!string.IsNullOrEmpty(message.Text) || !string.IsNullOrEmpty(message.Caption)))
                {
                    var messageDate = _dateTimeService.GetDateTimeWithOffset(message.Date);
                    var text = message.Text;
                    if (string.IsNullOrEmpty(message.Text))
                    {
                        text = message.Caption;
                    }

                    var time = TimeParserHelper.ParseTime(text);
                    var totalTime = TimeSpan.Zero;
                    
                    if (time != TimeSpan.Zero)
                    {
                        var counterDto = await _counterService.CountAndSave(
                            chatId.ToString(),
                            time,
                            message.From,
                            messageDate);
                        totalTime = counterDto.Today;
                    }
                    
                    await _messagesStore.Save(chatId.ToString(), new MessageLog
                    {
                        UserId = message.From.Id,
                        MessageId = message.MessageId,
                        MessageDate = messageDate,
                        Text = text,
                        Time = time,
                        TotalTime = totalTime,
                    });
                }
            }
        } 
        else if (update.Type == UpdateType.EditedMessage)
        {
            var editedMessage = update.EditedMessage;
            if (editedMessage != null && editedMessage?.Chat.Id != null)
            {
                var chatId = editedMessage?.Chat.Id;

                if (editedMessage.From != null &&
                    (!string.IsNullOrEmpty(editedMessage.Text) || !string.IsNullOrEmpty(editedMessage.Caption)))
                {
                    var messageDate = _dateTimeService.GetDateTimeWithOffset(editedMessage.Date);
                    var text = editedMessage.Text;
                    if (string.IsNullOrEmpty(editedMessage.Text))
                    {
                        text = editedMessage.Caption;
                    }

                    var loadedMessage = _messagesStore.Load(chatId.ToString(), editedMessage.MessageId);
                    if (loadedMessage != null)
                    {
                        var currentTime = TimeParserHelper.ParseTime(text);
                        var totalTime = TimeSpan.Zero; 
                            
                        var diffTime = currentTime - loadedMessage.Time;
                        if (diffTime != TimeSpan.Zero)
                        {
                            var counterDto = await _counterService.ReCountAndSave(
                                chatId.ToString(),
                                diffTime,
                                editedMessage.From);
                            totalTime = counterDto.Today;
                        }
                        
                        await _messagesStore.ReSave(chatId.ToString(), new MessageLog
                        {
                            UserId = editedMessage.From.Id,
                            MessageId = editedMessage.MessageId,
                            MessageDate = messageDate,
                            Text = text,
                            Time = currentTime,
                            TotalTime = totalTime,
                        });
                    }
                }
            }
        }
    }
}