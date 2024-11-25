using MeditationCountBot.Services;

namespace MeditationCountBot.Workers;

public class CounterWorker : BackgroundService
{
    private DateTime _lastSend = DateTime.MinValue;
    private readonly ICalculateResultService _calculateResultService;

    public CounterWorker(ICalculateResultService calculateResultService)
    {
        _calculateResultService = calculateResultService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
        while (!stoppingToken.IsCancellationRequested)
        {
            await _calculateResultService.CalculateTotalResultsAndSend();
            
            await Task.Delay(60 * 60000, stoppingToken);
        }
    }
}