using MeditationCountBot.Dto;

namespace MeditationCountBot.Services;

public interface IJsonLoader
{
    Task<Dictionary<string, CounterDto>> LoadAllJsons();
    Task SaveToJsonAsync(CounterDto counterDto, bool log = false);
}