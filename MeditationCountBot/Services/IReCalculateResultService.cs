namespace MeditationCountBot.Services;

public interface IReCalculateResultService
{
    Task<List<string>> ReCalculate(string logFile, DateTime date);
}