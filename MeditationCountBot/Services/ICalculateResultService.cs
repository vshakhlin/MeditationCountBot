namespace MeditationCountBot.Services;

public interface ICalculateResultService
{
    Task<List<string>> CalculateTotalResultsAndSend();
}