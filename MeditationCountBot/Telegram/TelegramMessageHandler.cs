using System.Text.Json;
using MeditationCountBot.Dto;
using MeditationCountBot.Services;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MeditationCountBot.Telegram;

public class TelegramMessageHandler : ITelegramMessageHandler
{
    private readonly ILogger<TelegramMessageHandler> _logger;
    private readonly ITelegramBotService _telegramBotService;
    private readonly ICounterService _counterService;
    private readonly IMessagesStore _messagesStore;

    public TelegramMessageHandler(
        ITelegramBotService telegramBotService,
        ICounterService counterService,
        ILogger<TelegramMessageHandler> logger,
        IMessagesStore messagesStore)
    {
        _telegramBotService = telegramBotService;
        _counterService = counterService;
        _logger = logger;
        _messagesStore = messagesStore;
    }

    public Task HandleErrorAsync(Exception exception, HandleErrorSource source)
    {
        _logger.LogError(JsonSerializer.Serialize(exception));
        return Task.CompletedTask;
    }

    public async Task HandleMessageAsync(Message msg, UpdateType type)
    {
        _logger.LogInformation(JsonSerializer.Serialize(msg));
        try
        {
            if (type == UpdateType.Message)
            {
                var message = msg;
                var language = message?.From?.LanguageCode ?? "en";
                var chatId = message?.Chat.Id;
                if (chatId != null)
                {
                    if (message.Chat.Type == ChatType.Private && !string.IsNullOrEmpty(message.Text) &&
                        (message.Text.ToLower() == "/help" || message.Text.ToLower() == "/start"))
                    {
                        await _telegramBotService.SendStartOrHelpMessageAsync(
                            chatId.Value,
                            "Help",
                            language,
                            cancellationToken: new CancellationToken());

                        return;
                    }

                    if (!string.IsNullOrEmpty(message.Text) &&
                        message.Text.ToLower().Contains(_telegramBotService.BotUsername) &&
                        message.Text.ToLower().Contains("/help"))
                    {
                        var admins = await _telegramBotService.GetChatAdministratorsAsync(message.Chat.Id, new CancellationToken());
                        if (admins.Any(cm => cm.User.Id == message.From.Id))
                        {
                            await _telegramBotService.SendInstructionMessageAsync(
                                chatId.Value,
                                cancellationToken: new CancellationToken(),
                                message.MessageId);
                        }

                        return;
                    }

                    if (!string.IsNullOrEmpty(message.Text) &&
                        message.Text.ToLower().Contains(_telegramBotService.BotUsername) &&
                        message.Text.ToLower().Contains("/timezone"))
                    {
                        var admins = await _telegramBotService.GetChatAdministratorsAsync(message.Chat.Id, new CancellationToken());
                        if (admins.Any(cm => cm.User.Id == message.From.Id))
                        {
                            var replaceCommand = message.Text.ToLower()
                                .Replace("@", string.Empty)
                                .Replace(_telegramBotService.BotUsername, string.Empty)
                                .Replace("/timezone", string.Empty)
                                .Trim();
                            if (int.TryParse(replaceCommand, out var timeZoneHour))
                            {
                                if (timeZoneHour >= -12 && timeZoneHour <= 12)
                                {
                                    await _counterService.UpdateSettings(chatId.ToString(), new SettingsDto()
                                    {
                                        TimeZone = TimeSpan.FromHours(timeZoneHour),
                                    });

                                    await _telegramBotService.SendTextMessageAsync(
                                        chatId.Value,
                                        "Настройки успешно сохранены",
                                        "ru",
                                        null,
                                        false,
                                        new CancellationToken());
                                }
                            }
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

                    if (message.From != null && !message.From.IsBot && (!string.IsNullOrEmpty(message.Text) ||
                                                                        !string.IsNullOrEmpty(message.Caption)))
                    {
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
                                message.Date);
                            totalTime = counterDto.Today;
                        }

                        await _messagesStore.Save(chatId.ToString(), new MessageLog
                        {
                            UserId = message.From.Id,
                            MessageId = message.MessageId,
                            MessageDate = message.Date,
                            Text = text,
                            Time = time,
                            TotalTime = totalTime,
                        });
                    }
                }
            }
            else if (type == UpdateType.EditedMessage)
            {
                var editedMessage = msg;
                if (editedMessage != null && editedMessage?.Chat.Id != null)
                {
                    var chatId = editedMessage?.Chat.Id;

                    if (editedMessage.From != null &&
                        (!string.IsNullOrEmpty(editedMessage.Text) || !string.IsNullOrEmpty(editedMessage.Caption)))
                    {
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
                                MessageDate = editedMessage.Date,
                                Text = text,
                                Time = currentTime,
                                TotalTime = totalTime,
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling update");
        }
    }
}