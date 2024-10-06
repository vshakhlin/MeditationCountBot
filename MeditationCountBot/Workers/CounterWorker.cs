using MeditationCountBot.Services;
using MeditationCountBot.Telegram;

namespace MeditationCountBot.Workers;

public class CounterWorker : BackgroundService
{
    private DateTime _lastSend = DateTime.MinValue;
    private readonly IJsonLoader _jsonLoader;
    private readonly ITelegramMessageSender _messageSender;
    private readonly IMessageFormer _messageFormer;
    private readonly ICalculateContinuouslyService _calculateContinuouslyService;

    public CounterWorker(IJsonLoader jsonLoader, ITelegramMessageSender messageSender, IMessageFormer messageFormer, ICalculateContinuouslyService calculateContinuouslyService)
    {
        _jsonLoader = jsonLoader;
        _messageSender = messageSender;
        _messageFormer = messageFormer;
        _calculateContinuouslyService = calculateContinuouslyService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow.AddHours(3);
            if (now.Hour >= 23 && now.Minute >= 55 && _lastSend.Date < now.Date)
            {
                Console.WriteLine($"Start calculation {now}");
                _lastSend = now;

                var dict = await _jsonLoader.LoadAllJsons();
                Console.WriteLine($"Count: {dict.Count}");
                foreach (var counter in dict.Values)
                {
                    _calculateContinuouslyService.CalculateContinuouslyDays(counter, DateTime.UtcNow);
                    var message = _messageFormer.CreateMessage(counter);
                    Console.WriteLine(message);

                    counter.Yesterday = counter.Today;
                    if (counter.Best < counter.Today)
                    {
                        counter.Best = counter.Today;
                    }
                
                    counter.Today = TimeSpan.Zero;
                    await _jsonLoader.SaveToJsonAsync(counter, true);
                    Console.WriteLine($"SAVED");
                    
                    var sendResult = await _messageSender.SendMessage(counter.ChatId, message, "ru", null, new CancellationToken());
                    Console.WriteLine($"SEND: {sendResult.IsSuccess}");
                    if (!string.IsNullOrEmpty(sendResult.ErrorMessage))
                    {
                        Console.WriteLine($"Error: {sendResult.ErrorMessage}");
                    }
                }
            }
            
            await Task.Delay(60000, stoppingToken);
        }
    }
}