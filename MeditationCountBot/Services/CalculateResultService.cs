using MeditationCountBot.Dto;
using MeditationCountBot.Telegram;

namespace MeditationCountBot.Services;

public class CalculateResultService : ICalculateResultService
{
    private readonly IJsonLoader _jsonLoader;
    private readonly IMessagesStore _messagesStore;
    private readonly ITelegramMessageSender _messageSender;
    private readonly IMessageFormer _messageFormer;
    private readonly ICounterService _counterService;
    private readonly ICalculateContinuouslyService _calculateContinuouslyService;
    private readonly IDateTimeService _dateTimeService;
    private readonly ILogger<CalculateResultService> _logger;
    
    public CalculateResultService(
        IMessageFormer messageFormer,
        IJsonLoader jsonLoader,
        ITelegramMessageSender messageSender,
        ICounterService counterService,
        ICalculateContinuouslyService calculateContinuouslyService,
        IMessagesStore messagesStore,
        IDateTimeService dateTimeService,
        ILogger<CalculateResultService> logger)
    {
        _messageFormer = messageFormer;
        _jsonLoader = jsonLoader;
        _messageSender = messageSender;
        _counterService = counterService;
        _calculateContinuouslyService = calculateContinuouslyService;
        _messagesStore = messagesStore;
        _dateTimeService = dateTimeService;
        _logger = logger;
    }
    
    public async Task<List<string>> CalculateTotalResultsAndSend()
    {
        _logger.LogInformation("Start backup");
        
        _logger.LogInformation($"Start calculation {_dateTimeService.GetDateTimeUtcNow()}");

        var results = new List<string>();
        var dict = await _jsonLoader.LoadAllJsons<CounterDto>(JsonLoader.ChatsPath);
        _logger.LogInformation($"Count: {dict.Count}");
        foreach (var counter in dict.Values)
        {
            if (counter.Today.TotalMinutes <= 0)
            {
                continue;
            }
            
            var now = _dateTimeService.GetDateTimeNow(counter.Settings.TimeZone);
            if (now.Hour != 0)
            {
                continue;
            }
            
            await _jsonLoader.Backup(counter.ChatId, counter);

            _calculateContinuouslyService.CalculateContinuouslyDays(counter, now);
            var message = _messageFormer.CreateMessage(counter);
            _logger.LogInformation(message);
            
            counter.Total += counter.Today;
            counter.Yesterday = counter.Today;
            if (counter.Best < counter.Today)
            {
                counter.Best = counter.Today;
            }
            
            counter.Today = TimeSpan.Zero;
            await _jsonLoader.SaveToJsonAsync(counter.ChatId, counter, JsonLoader.ChatsPath, true);
            _logger.LogInformation($"SAVED");
                
            var sendResult = await _messageSender.SendMessage(counter.ChatId, message, "ru", null, new CancellationToken());
            _logger.LogInformation($"SEND: {sendResult.IsSuccess}");
            var sendResultMessage = "";
            if (!string.IsNullOrEmpty(sendResult.ErrorMessage))
            {
                sendResultMessage = $"Error: {sendResult.ErrorMessage}";
            }
            else
            {
                sendResultMessage = $"Send {counter.ChatId} at {_dateTimeService.GetDateTimeUtcNow()}";
            }
            
            _logger.LogInformation(sendResultMessage);
            results.Add(sendResultMessage);
            
            await _messagesStore.Clear(counter.ChatId);
        }
        
        await _counterService.Reload();
        await _messagesStore.Reload();

        return results;
    }
}