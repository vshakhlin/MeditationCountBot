using MeditationCountBot.Services;

namespace MeditationCountBot.Workers;

public class CounterWorker : BackgroundService
{
    private DateTime _lastSend = DateTime.MinValue;
    private readonly IDateTimeService _dateTimeService;
    private readonly ICalculateResultService _calculateResultService;

    public CounterWorker(
        IDateTimeService dateTimeService, ICalculateResultService calculateResultService)
    {
        _dateTimeService = dateTimeService;
        _calculateResultService = calculateResultService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = _dateTimeService.GetDateTimeNow();
            if (now.Hour >= 23 && now.Minute >= 55 && _lastSend.Date < now.Date)
            {
                await _calculateResultService.CalculateTotalResultsAndSend();
            }
            
            await Task.Delay(60000, stoppingToken);
        }
    }
}